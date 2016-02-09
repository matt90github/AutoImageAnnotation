// This program consists of three main parts:
// 1. Building a visual vocabulary from a subset of the training images by extracting SIFT feature points and computing the descriptor for each point, followed by K-Means clustering
// 2. Extracting SIFT feature points and computing descriptors for all the images in the training set, matching the descriptors with the vocabulary defined earlier and extracting a histogram
// 3. Training a 'one-against-all' SVM classifier for each defined concept based on the extracted features and labels associated with each image
// Note: This program is based on the implementation by Ravimal Bandara available at: http://www.codeproject.com/Tips/656906/Bag-of-Features-Descriptor-on-SURF-and-ORB-Feature

//Including other source files and namespaces that are required for the successful execution of the program
#include "stdafx.h"
#include <opencv2/opencv.hpp>
#include <opencv/cv.h>
#include <opencv/highgui.h>
#include <opencv2/features2d/features2d.hpp>
#include <opencv2/nonfree/features2d.hpp>
#include <conio.h>
#include "tinyxml2.h"

using namespace cv;
using namespace std;
using namespace tinyxml2;

//When the GenerateVocabulary constant is set to 1, the first section of the program is executed
//Otherwise, sections 2 and 3 are executed
#define GenerateVocabulary 1

//The program's main method
int _tmain(int argc, _TCHAR* argv[])
{
	//Load the XML Configuration File
	XMLDocument applicationConfig;
	applicationConfig.LoadFile("appconfig.xml");

	//Defining the number of 'bags of features' to be extracted
	int bagsOfFeatures = atoi(applicationConfig.FirstChildElement("APPCONFIG")->FirstChildElement("BagsOfFeatures")->GetText());

	//Initialising the number of training images that will be used for vocabulary generation
	int vocabularyImages = atoi(applicationConfig.FirstChildElement("APPCONFIG")->FirstChildElement("VocabularyImages")->GetText());

	//Initialising the number of training images per concept
	int trainingImages = atoi(applicationConfig.FirstChildElement("APPCONFIG")->FirstChildElement("TrainingImagesPerConcept")->GetText());

	//Initialising the default directory to be used throughout the application
	const char * baseDirectory = applicationConfig.FirstChildElement("APPCONFIG")->FirstChildElement("BaseDirectory")->GetText();

	//Holds the path to the location of the training image file
	char * trainingImageFileName = new char[1000];

	//Holds the path to the location of the SVM trained model file
	char * trainedModelFilename = new char[1000];

	//Holds the path to the location of the generated vocabulary file
	char * generatedVocabularyFilename = new char[1000];

#if GenerateVocabulary == 1

	//Display current status on the console window
	printf("%s\n", "-------------------------------------------------------------------");
	printf("%s\n", "------ Feature Extraction Application: Vocabulary Generation ------");
	printf("%s\n\n", "-------------------------------------------------------------------");

	//A matrix to hold the current image being processed
	Mat currentImage;

	//A vector that will be populated with SIFT feature points extracted from the current image
	vector<KeyPoint> featurePoints;

	//A matrix to hold the SIFT feature descriptor of the current image
	Mat featureDescriptor;

	//A matrix to hold all the SIFT feature descriptors from all the images in the training set subset
	Mat combinedFeatureDescriptors;

	//Initialising the SIFT feature detector and descriptor
	SiftDescriptorExtractor detector;

	//Looping through the subset of the training images
	for (int i = 1; i <= vocabularyImages; i++){

		//Populate the location where the training image is currently stored
		sprintf(trainingImageFileName, "%s\\vocabulary\\%i.jpg", baseDirectory, i);

		//Load the grayscale version of the image into the matrix
		currentImage = imread(trainingImageFileName, CV_LOAD_IMAGE_GRAYSCALE);

		//Check for images having a .jpeg extension
		if (currentImage.empty())
		{
			sprintf(trainingImageFileName, "%s\\vocabulary\\%i.jpeg", baseDirectory, i);
			currentImage = imread(trainingImageFileName, CV_LOAD_IMAGE_GRAYSCALE);
		}

		//If the image exists
		if (!currentImage.empty())
		{
			//Get the SIFT feature points from the current image
			detector.detect(currentImage, featurePoints);

			//Get the SIFT feature descriptor for the extracted feature points
			detector.compute(currentImage, featurePoints, featureDescriptor);

			//Combine all the feature descriptors in one matrix
			combinedFeatureDescriptors.push_back(featureDescriptor);

			//Show the progess in the console window
			printf("Processed Vocabulary Image %i/%i\n", i, vocabularyImages);

			//Release the memory resources associated with the current image
			currentImage.release();
		}
	}

	//Display current status on the console window
	printf("\n");
	printf("Clustering and generating vocabulary...\n");

	//Initialise the termination criteria of the K-Means algorithm
	TermCriteria terminationCriteria(CV_TERMCRIT_ITER, 100, 0.001);

	//The number of attempts
	int attempts = 1;

	//Uses K-Means++ algorithm
	int flags = KMEANS_PP_CENTERS;

	//Initialising a K-means-based class to train visual vocabulary using the bag of visual words approach
	BOWKMeansTrainer trainer(bagsOfFeatures, terminationCriteria, attempts, flags);

	//Generate the vocabulary by clustering the combined feature descriptors using the K-Means algorithm
	Mat vocabulary = trainer.cluster(combinedFeatureDescriptors);

	//Store the generated vocabulary
	sprintf(generatedVocabularyFilename, "%s\\vocabulary\\generated\\vocabulary.yml", baseDirectory);
	FileStorage vocabularyStorage(generatedVocabularyFilename, FileStorage::WRITE);
	vocabularyStorage << "vocabulary" << vocabulary;

	//Release the memory resources associated with the file storage
	vocabularyStorage.release();

	//Display current status on the console window
	printf("\n");
	printf("Vocabulary successfully generated and saved!\n");

#else
	//Display current status on the console window
	printf("%s\n", "---------------------------------------------------------------------");
	printf("%s\n", "------ Feature Detection and Description & Classifier Training ------");
	printf("%s\n", "---------------------------------------------------------------------");

	//A matrix to hold the vocabulary generated in the first section of the program
	Mat vocabulary;

	//Load the vocabulary file
	sprintf(generatedVocabularyFilename, "%s\\vocabulary\\generated\\vocabulary.yml", baseDirectory);
	FileStorage vocabularyStorage(generatedVocabularyFilename, FileStorage::READ);

	//Populate the matrix
	vocabularyStorage["vocabulary"] >> vocabulary;

	//Release the memory resources associated with the file storage
	vocabularyStorage.release();

	//Abstract base class for matching keypoint descriptors based on the FLANN (Fast Approximate Nearest Neighbor Search Library) algorithm
	Ptr<DescriptorMatcher> featureDescriptorMatcher(new FlannBasedMatcher);

	//Initialise the SIFT feature detector
	Ptr<FeatureDetector> featureDetector(new SiftFeatureDetector());

	//Initialise the SIFT feature descriptor
	Ptr<DescriptorExtractor> featureDescriptor(new SiftDescriptorExtractor);

	//Initialise the Bag of Words Descriptor Extractor
	BOWImgDescriptorExtractor descriptorExtractor(featureDescriptor, featureDescriptorMatcher);

	//Set the vocabulary of the descriptor extractor to the one generated earlier
	descriptorExtractor.setVocabulary(vocabulary);

	//A matrix storing the labels provided to the Support Vector Machine classifier for both positive (+1) and negative (-1) images of each concept
	Mat trainingLabels(0, 1, CV_32FC1);

	//A matrix storing the Bag of Words (BoW) representation of the features of each image
	Mat trainingData(0, bagsOfFeatures, CV_32FC1);

	//A vector that will be populated with SIFT feature points extracted from the current image
	vector<KeyPoint> featurePoints;

	//A matrix to hold the Bag of Words (BoW) representation (histogram) of the current image
	Mat bagOfWordsRepresentation;

	//Retrieving the list of comma-separated concepts from the configuration file, parsing them and placing them in a string vector
	const char * conceptList = applicationConfig.FirstChildElement("APPCONFIG")->FirstChildElement("CustomConcepts")->GetText();
	string conceptListStr = string(conceptList);
	std::stringstream streamOfConcepts(conceptListStr);

	std::vector<std::string> concepts = {};

	while (streamOfConcepts.good())
	{
		string singleConcept;
		getline(streamOfConcepts, singleConcept, ',');
		concepts.push_back(singleConcept);
	}

	//Loop through each defined concept
	for each (string concept in concepts)
	{
		//Displays the current status in the console window
		printf("\n");
		printf("-------------------\n");
		printf("Started Concept: %s\n\n", concept.c_str());

		//Process both negative and positive images for each concept
		for (int z = -1; z < 2; z += 2) {
			for (int i = 1; i <= trainingImages; i++){

				//Populate the location where the training image is currently stored
				sprintf(trainingImageFileName, "%s\\trainingimages\\%s\\%i(%i).jpg", baseDirectory, concept.c_str(), z, i);

				//Load the grayscale version of the image into the matrix
				Mat currentImage = imread(trainingImageFileName, CV_LOAD_IMAGE_GRAYSCALE);

				//Check for images having a .jpeg extension
				if (currentImage.empty())
				{
					sprintf(trainingImageFileName, "%s\\trainingimages\\%s\\%i(%i).jpeg", baseDirectory, concept.c_str(), z, i);
					currentImage = imread(trainingImageFileName, CV_LOAD_IMAGE_GRAYSCALE);
				}

				//If the image exists
				if (!currentImage.empty())
				{
					//Get the SIFT feature points from the current image
					featureDetector->detect(currentImage, featurePoints);

					//Get the Bag of Words (BoW) representation (histogram) of the current image
					descriptorExtractor.compute(currentImage, featurePoints, bagOfWordsRepresentation);

					//Add the training label of the current image (+1/-1)
					trainingLabels.push_back((float)z);

					//Add the Bag of Words representation as part of the training data (to be supplied to the SVM)
					trainingData.push_back(bagOfWordsRepresentation);

					//Display the image processing status on the console window
					if (z == 1)
						printf("Processed Positive Image %i\n", i);
					else
						printf("Processed Negative Image %i\n", i);

					//Release the memory resources associated with the current image
					currentImage.release();
				}
			}
		}

		//Display current status on the console window
		printf("\n");
		printf("Training SVM Classifier for concept %s...\n", concept.c_str());

		//The SVM instance for the particular concept
		CvSVM svm;

		//To store the SVM training parameters
		CvSVMParams trainingParams;

		//Set SVM type to C-Support Vector Classification (C_SVC): n-class classification(n>=2), allows imperfect separation of classes with penalty multiplier C for outliers.
		trainingParams.svm_type = CvSVM::C_SVC;

		//Make use of the Radial basis function (RBF) SVM kernel
		trainingParams.kernel_type = CvSVM::RBF;

		//Defining the termination criteria
		trainingParams.term_crit = cvTermCriteria(CV_TERMCRIT_ITER, 100, 0.000001);

		//Setting up the SVM training parameters C and gamma
		CvParamGrid CvParamGrid_C(pow(2.0, -5), pow(2.0, 15), pow(2.0, 2));
		CvParamGrid CvParamGrid_gamma(pow(2.0, -15), pow(2.0, 3), pow(2.0, 2));

		//The method trains the SVM model automatically by choosing the optimal parameters C, gamma, p, nu, coef0, degree from CvSVMParams.
		//Parameters are considered optimal when the cross-validation estimate of the test set error is minimal.
		bool res = svm.train_auto(trainingData, trainingLabels, Mat(), Mat(), trainingParams, 10, CvParamGrid_C, CvParamGrid_gamma,
			CvSVM::get_default_grid(CvSVM::P), CvSVM::get_default_grid(CvSVM::NU), CvSVM::get_default_grid(CvSVM::COEF),
			CvSVM::get_default_grid(CvSVM::DEGREE), true);

		//Display current status on the console window
		printf("\n");
		printf("Optimal SVM parameters for current model:\n");

		//Retrieve the SVM training parameters
		trainingParams = svm.get_params();
		cout << "gamma:" << trainingParams.gamma << endl;
		cout << "C:" << trainingParams.C << endl;

		//Populate the location where the trained SVM model will be stored
		sprintf(trainedModelFilename, "%s\\svm_models\\svm_trainedmodel_%s.xml", baseDirectory, concept.c_str());

		//Save the trained SVM model for the current concept
		svm.save(trainedModelFilename);

		//Release the associated resources
		svm.clear();
		trainingData.release();
		trainingLabels.release();

		//Display current status on the console window
		printf("\n");
		printf("SVM model successfully generated!\n");
	}
#endif

	//Display final status on the console window
	printf("\nDone\n");

	//Wait for user input to close the application
	_getch();

	return 0;
}