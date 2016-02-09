// This program uses the SIFT feature detector and descriptor to extract features from a number of testing images, after which 
// they are presented to the 20 trained SVM models for automatic image annotation

//Including other source files and namespaces that are required for the successful execution of the program
#include "stdafx.h"
#include <opencv2/opencv.hpp>
#include <opencv/cv.h>
#include <opencv/highgui.h>
#include <opencv2/features2d/features2d.hpp>
#include <opencv2/nonfree/features2d.hpp>
#include <conio.h>
#include <fstream>
#include "tinyxml2.h"

using namespace cv;
using namespace std;
using namespace tinyxml2;

//The program's main method
int _tmain(int argc, _TCHAR* argv[])
{
	//Display current status on the console window
	printf("%s\n", "------------------------------------------");
	printf("%s\n", "------ Image Annotation Application ------");
	printf("%s\n\n", "------------------------------------------");

	//Load the XML Configuration File
	XMLDocument applicationConfig;
	applicationConfig.LoadFile("appconfig.xml");

	//Defining the number of 'bags of features' to be extracted
	int bagsOfFeatures = atoi(applicationConfig.FirstChildElement("APPCONFIG")->FirstChildElement("BagsOfFeatures")->GetText());

	//Initialising the number of testing images per concept
	int testingImages = atoi(applicationConfig.FirstChildElement("APPCONFIG")->FirstChildElement("TestingImagesPerConcept")->GetText());

	//Initialising the default directory to be used throughout the application
	const char * baseDirectory = applicationConfig.FirstChildElement("APPCONFIG")->FirstChildElement("BaseDirectory")->GetText();

	//Holds the path to the location of the generated vocabulary file
	char * generatedVocabularyFilename = new char[1000];

	//Holds the path to the location of the test image file
	char * testingImageFileName = new char[1000];

	//Holds the path to the location of the SVM trained model file
	char * trainedModelFilename = new char[1000];

	//Holds the path to the location where the SVM prediction results will be stored
	char * resultsPath = new char[1000];

	//Holds the path to the location where the annotation results will be stored
	char * annotationFileName = new char[1000];

	//Display current status on the console window
	printf("%s\n", "Processing testing images...");

	//A matrix to hold the already generated vocabulary
	Mat vocabulary;

	//Load the vocabulary file
	sprintf(generatedVocabularyFilename, "%s\\vocabulary\\generated\\vocabulary.yml", baseDirectory);
	FileStorage vocabularyStorage(generatedVocabularyFilename, FileStorage::READ);

	//Populate the matrix
	vocabularyStorage["vocabulary"] >> vocabulary;

	//Release the memory resources associated with the file storage
	vocabularyStorage.release();

	//A matrix storing the Bag of Words (BoW) representation of the features of each test image
	Mat testingData(0, bagsOfFeatures, CV_32FC1);

	//An output file stream used to save the annotation results to a text file
	std::ofstream annotationResults;

	//A vector that will be populated with SIFT feature points extracted from the current image
	vector<KeyPoint> featurePoints;

	//A matrix to hold the Bag of Words (BoW) representation (histogram) of the current image
	Mat bagOfWordsRepresentation;

	//A matrix to hold the classification results of each trained SVM model
	Mat modelResults(0, 1, CV_32FC1);

	//Abstract base class for matching keypoint descriptors based on the FLANN (Fast Approximate Nearest Neighbor Search Library) algorithm
	Ptr<DescriptorMatcher> featureDescriptorMatcher(new FlannBasedMatcher);

	//Initialise the SIFT feature detector
	Ptr<FeatureDetector> featureDetector(new SiftFeatureDetector());

	//Initialise the SIFT feature descriptor
	Ptr<DescriptorExtractor> featureDescriptor(new SiftDescriptorExtractor);

	//Initialise the Bag of Words Descriptor Extractor
	BOWImgDescriptorExtractor descriptorExtractor(featureDescriptor, featureDescriptorMatcher);

	//Set the dictionary with the vocabulary we created in the first step
	descriptorExtractor.setVocabulary(vocabulary);

	//The SVM instance
	CvSVM svm;

	//An array storing the correct annotations per concept
	double correctConceptAnnotation[20] = { 0 };

	//An array storing the correct SVM labels per concept
	double truePositiveAnnotation[20] = { 0 };

	//An array storing the false positive SVM labels per concept
	double falsePositiveAnnotation[20] = { 0 };

	//An array storing the true negative SVM labels per concept
	double trueNegativeAnnotation[20] = { 0 };

	//An array storing the false negative SVM labels per concept
	double falseNegativeAnnotation[20] = { 0 };

	//Open the text file to be used to store the annotation results
	sprintf(annotationFileName, "%s\\annotations\\annotations.txt", baseDirectory);
	annotationResults.open(annotationFileName);

	//A counter to keep track of how many concepts were covered so far
	int conceptCounter = 0;

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

	//Looping through each defined concept
	for each (string concept in concepts)
	{
		//Process each test image
		for (int i = 1; i <= testingImages; i++){

			//Populate the location where the test image is currently stored
			sprintf(testingImageFileName, "%s\\testingimages\\%s\\%i.jpg", baseDirectory, concept.c_str(), i);

			//Load the grayscale version of the image into the matrix
			Mat currentImage = imread(testingImageFileName, CV_LOAD_IMAGE_GRAYSCALE);

			//Check for images having a .jpeg extension
			if (currentImage.empty())
			{
				sprintf(testingImageFileName, "%s\\testingimages\\%s\\%i.jpeg", baseDirectory, concept.c_str(), i);
				currentImage = imread(testingImageFileName, CV_LOAD_IMAGE_GRAYSCALE);
			}

			//If the image exists
			if (!currentImage.empty())
			{
				//Get the SIFT feature points from the current image
				featureDetector->detect(currentImage, featurePoints);

				//Get the Bag of Words (BoW) representation (histogram) of the current image
				descriptorExtractor.compute(currentImage, featurePoints, bagOfWordsRepresentation);

				//Add the Bag of Words representation to the testing matrix
				testingData.push_back(bagOfWordsRepresentation);

				//Stores the classification result from the SVM (label)
				float svmLabel = 0;

				//Stores the decision function value that is signed distance to the margin
				float marginDistance = 0;

				//Stores the confidence score
				float confidence = 0;

				//Two arrays storing the original and sorted confidence scores (per concept) for each image
				float originalConfidenceScore[20] = { 0 }, confidenceScore[20] = { 0 };

				//Another counter to keep track of how many concepts were covered so far
				int innerConceptCounter = 0;

				for each (string innerConcept in concepts)
				{
					//Populate the location where the trained SVM model is currently stored
					sprintf(trainedModelFilename, "%s\\svm_models\\svm_trainedmodel_%s.xml", baseDirectory, innerConcept.c_str());

					//Load the trained SVM model
					svm.load(trainedModelFilename);

					//Get the SVM classification label for the image
					svmLabel = svm.predict(bagOfWordsRepresentation);

					if (innerConcept == concepts[conceptCounter] && svmLabel == 1)
						truePositiveAnnotation[conceptCounter]++;

					if (innerConcept != concepts[conceptCounter] && svmLabel == 1)
						falsePositiveAnnotation[conceptCounter]++;

					if (innerConcept == concepts[conceptCounter] && svmLabel == -1)
						falseNegativeAnnotation[conceptCounter]++;

					if (innerConcept != concepts[conceptCounter] && svmLabel == -1)
						trueNegativeAnnotation[conceptCounter]++;

					//Get the signed distance to the margin
					marginDistance = svm.predict(bagOfWordsRepresentation, true);

					//Compute the confidence score by performing the Sigmoid function
					confidence = 1.0 / (1.0 + exp(-marginDistance));

					//Populate the confidence scores
					originalConfidenceScore[innerConceptCounter] = confidence;
					confidenceScore[innerConceptCounter] = confidence;

					//Add the classification results 
					modelResults.push_back(svmLabel);

					//Increment the concept counter
					innerConceptCounter++;

					//Clear the SVM to prepare for the next trained model
					svm.clear();
				}

				//Sort the confidence scores in ascending order to get the closest match
				std::sort(confidenceScore, confidenceScore + 20);

				//A string storing the detected concept name
				string detectedAnnotation = "";

				//Mapping the confidence score back to the concept name, and checking if the annotation is the correct one
				for (int i = 0; i < 20; i++){
					if (confidenceScore[0] == originalConfidenceScore[i])
					{
						detectedAnnotation = concepts[i];

						if (concepts[i] == concept)
							correctConceptAnnotation[conceptCounter]++;

						break;
					}
				}

				//Outputting the detected annotation to the console window and text file
				std::cout << "Annotation " << i + (conceptCounter * testingImages) << ": " << detectedAnnotation << "\n";
				annotationResults << "Annotation " << i + (conceptCounter * testingImages) << ": " << detectedAnnotation << "\n";

				//Release the memory resources associated with the current image
				currentImage.release();
			}
		}

		//Populate the location where the SVM classification results will be stored
		sprintf(resultsPath, "%s\\svm_results\\svm_results_%s.yml", baseDirectory, concept.c_str());

		//Open the results file
		FileStorage resultsStorage(resultsPath, FileStorage::WRITE);

		//Add the model results
		resultsStorage << "model results" << modelResults;

		//Release the memory resources associated with the file storage
		resultsStorage.release();

		//Release the associated resources
		testingData.release();
		modelResults.release();

		//Increment the concept counter
		conceptCounter++;
	}

	conceptCounter = 0;
	double totalRecognised = 0, totalCorrectlyClassified = 0, totalFalseNegatives = 0;

	for each (string concept in concepts)
	{
		//Compute the accuracy rate per concept
		totalCorrectlyClassified += truePositiveAnnotation[conceptCounter];
		float labelAccuracyPercentage = ((float)(truePositiveAnnotation[conceptCounter] / testingImages) * 100);
		float labelPercentageRoundedDown = floorf(labelAccuracyPercentage * 100) / 100;

		totalRecognised += correctConceptAnnotation[conceptCounter];
		float annotationAccuracyPercentage = ((float)(correctConceptAnnotation[conceptCounter] / testingImages) * 100);
		float annotationPercentageRoundedDown = floorf(annotationAccuracyPercentage * 100) / 100;

		totalFalseNegatives += falseNegativeAnnotation[conceptCounter];
		float falseNegativePercentage = ((float)(falseNegativeAnnotation[conceptCounter] / testingImages) * 100);
		float falseNegativePercentageRoundedDown = floorf(falseNegativePercentage * 100) / 100;

		float accuracyPercentage = ((float)((truePositiveAnnotation[conceptCounter] + trueNegativeAnnotation[conceptCounter]) /
			(truePositiveAnnotation[conceptCounter] + trueNegativeAnnotation[conceptCounter] +
			falsePositiveAnnotation[conceptCounter] + falseNegativeAnnotation[conceptCounter])) * 100);
		float accuracyPercentageRoundedDown = floorf(accuracyPercentage * 100) / 100;

		float precisionPercentage = ((float)(truePositiveAnnotation[conceptCounter] /
			(truePositiveAnnotation[conceptCounter] + falsePositiveAnnotation[conceptCounter]) * 100));
		float precisionPercentageRoundedDown = floorf(precisionPercentage * 100) / 100;

		float recallPercentage = ((float)(truePositiveAnnotation[conceptCounter] /
			(truePositiveAnnotation[conceptCounter] + falseNegativeAnnotation[conceptCounter]) * 100));
		float recallPercentageRoundedDown = floorf(recallPercentage * 100) / 100;

		//Display the results for the SVM model
		std::cout << "\n\n-----------------------------------------------------------------";
		annotationResults << "\n\n-----------------------------------------------------------------";

		std::cout << "\n\nModel results for concept '" << concept << "' (1000 images): ";
		annotationResults << "\n\nModel results for concept '" << concept << "' (1000 images): ";

		std::cout << "\n\nTrue Positives: " << truePositiveAnnotation[conceptCounter] << "/" << testingImages << " (" << labelPercentageRoundedDown << "%)";
		annotationResults << "\n\nTrue Positives: " << truePositiveAnnotation[conceptCounter] << "/" << testingImages << " (" << labelPercentageRoundedDown << "%)";

		std::cout << "\nFalse Negatives: " << falseNegativeAnnotation[conceptCounter] << "/" << testingImages << " (" << falseNegativePercentageRoundedDown << "%)";
		annotationResults << "\nFalse Negatives: " << falseNegativeAnnotation[conceptCounter] << "/" << testingImages << " (" << falseNegativePercentageRoundedDown << "%)";

		std::cout << "\nClosest Match: " << correctConceptAnnotation[conceptCounter] << "/" << testingImages << " (" << annotationPercentageRoundedDown << "%)";
		annotationResults << "\nClosest Match: " << correctConceptAnnotation[conceptCounter] << "/" << testingImages << " (" << annotationPercentageRoundedDown << "%)";

		std::cout << "\n\nTrue Negatives: " << trueNegativeAnnotation[conceptCounter];
		annotationResults << "\n\nTrue Negatives: " << trueNegativeAnnotation[conceptCounter];

		std::cout << "\nFalse Positives: " << falsePositiveAnnotation[conceptCounter];
		annotationResults << "\nFalse Positives: " << falsePositiveAnnotation[conceptCounter];

		std::cout << "\n\nAccuracy: " << accuracyPercentageRoundedDown << "%";
		annotationResults << "\n\nAccuracy: " << accuracyPercentageRoundedDown << "%";

		std::cout << "\nPrecision: " << precisionPercentageRoundedDown << "%";
		annotationResults << "\nPrecision: " << precisionPercentageRoundedDown << "%";

		std::cout << "\nRecall: " << recallPercentageRoundedDown << "%";
		annotationResults << "\nRecall: " << recallPercentageRoundedDown << "%";

		conceptCounter++;
	}

	std::cout << "\n\n-----------------------------------------------------------------";
	annotationResults << "\n\n-----------------------------------------------------------------";

	//Compute the overall correctly classified rate
	float overallLabelAccuracy = ((float)(totalCorrectlyClassified / (testingImages * concepts.size())) * 100);
	float overallLabelRounded = floorf(overallLabelAccuracy * 100) / 100;

	//Display the overall correctly classified rate
	std::cout << "\n\nTrue Positives (Overall): " << overallLabelRounded << "%";
	annotationResults << "\n\nTrue Positives (Overall): " << overallLabelRounded << "%";

	//Compute the overall accuracy rate
	float overallAccuracy = ((float)(totalRecognised / (testingImages * concepts.size())) * 100);
	float overallRounded = floorf(overallAccuracy * 100) / 100;

	//Display the overall accuracy rate
	std::cout << "\nClosest Match (Overall): " << overallRounded << "%";
	annotationResults << "\nClosest Match (Overall): " << overallRounded << "%";

	//Display final status on the console window
	printf("\n\nDone!\n");

	//Wait for user input to close the application
	_getch();

	return 0;
}

