//
//  BEHealthKit.m
//  Unity-iPhone
//
//  Created by greay on 3/25/15.
//
//

#import "BEHealthKit.h"
#import <HealthKit/HealthKit.h>
#import "BEHealthKit+read.h"
#import "BEHealthKit+write.h"
#import "NSError+XML.h"

// @cond INTERNAL
@interface BEHealthKit ()

@property (nonatomic, assign) NSUInteger anchor;

@end
// @endcond

// -----------------------------------

@implementation BEHealthKit

+ (instancetype)sharedHealthKit
{
	static dispatch_once_t onceToken;
	static BEHealthKit *healthKit = nil;
	dispatch_once(&onceToken, ^{
		healthKit = [[self alloc] init];
	});
	return healthKit;
}


- (instancetype)init {
	if (self = [super init]) {
		self.healthStore = [[HKHealthStore alloc] init];
		self.anchor = 0;
	}
	return self;
}

- (void)authorizeHealthKitToRead:(NSArray *)readIdentifiers write:(NSArray *)writeIdentifiers completion:(void (^)(bool success, NSError *error))completion {
	if ([HKHealthStore isHealthDataAvailable]) {
		NSSet *healthKitTypesToRead = [self dataTypes:readIdentifiers writePermission:false];
		NSSet *healthKitTypesToWrite = [self dataTypes:writeIdentifiers writePermission:true];
		[self.healthStore requestAuthorizationToShareTypes:healthKitTypesToWrite readTypes:healthKitTypesToRead completion:^(BOOL success, NSError *error) {
			if( completion != nil ) {
				completion(success, error);
			}
		}];
	} else {
		NSLog(@"error; Health Store is unavailable");
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_HK_UNAVAILABLE userInfo:@{NSLocalizedDescriptionKey:@"HealthKit is not available on this device."}];
		[self errorOccurred:err];
	}
}


- (int)authorizationStatusForType:(NSString *)dataTypeString {
	HKObjectType *datatype = [[self dataTypes:@[dataTypeString] writePermission:false] anyObject];
	HKAuthorizationStatus status = [self.healthStore authorizationStatusForType:datatype];
	return (int)status;
}


// ----------------------------
#pragma mark -
// ----------------------------


- (void)errorOccurred:(NSError *)error
{
	UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "HealthKitErrorOccurred", [[error XMLString] cStringUsingEncoding:NSUTF8StringEncoding]);
}


// ----------------------------
#pragma mark -
// ----------------------------

- (NSArray *)quantityTypes:(BOOL)write
{
	NSMutableArray *quantityTypes = [NSMutableArray array];
	
	// Body Measurements
	[quantityTypes addObjectsFromArray:@[
		HKQuantityTypeIdentifierBodyMassIndex,
		HKQuantityTypeIdentifierBodyFatPercentage,
		HKQuantityTypeIdentifierHeight,
		HKQuantityTypeIdentifierBodyMass,
		HKQuantityTypeIdentifierLeanBodyMass
	]];
	if (@available(iOS 11.0, *)) {
		[quantityTypes addObject: HKQuantityTypeIdentifierWaistCircumference];
	}
	
	// Fitness
	[quantityTypes addObjectsFromArray:@[
		HKQuantityTypeIdentifierStepCount,
		HKQuantityTypeIdentifierDistanceWalkingRunning,
		HKQuantityTypeIdentifierDistanceCycling,
		HKQuantityTypeIdentifierDistanceWheelchair,
		HKQuantityTypeIdentifierBasalEnergyBurned,
		HKQuantityTypeIdentifierActiveEnergyBurned,
		HKQuantityTypeIdentifierFlightsClimbed,
		HKQuantityTypeIdentifierPushCount,
		HKQuantityTypeIdentifierDistanceSwimming,
		HKQuantityTypeIdentifierSwimmingStrokeCount
	]];
	if (!write) {
		[quantityTypes addObjectsFromArray:@[
			HKQuantityTypeIdentifierNikeFuel,
			HKQuantityTypeIdentifierAppleExerciseTime
		]];
	}
	if (@available(iOS 11.0, *)) {
		[quantityTypes addObject:HKQuantityTypeIdentifierVO2Max];
	}
	if (@available(iOS 11.2, *)) {
		[quantityTypes addObject:HKQuantityTypeIdentifierDistanceDownhillSnowSports];
	}
	if (@available(iOS 13.0, *)) {
		[quantityTypes addObject:HKQuantityTypeIdentifierAppleStandTime];
	}
	if (@available(iOS 14.0, *)) {
		[quantityTypes addObjectsFromArray:@[
			HKQuantityTypeIdentifierWalkingSpeed,
			HKQuantityTypeIdentifierWalkingDoubleSupportPercentage,
			HKQuantityTypeIdentifierWalkingAsymmetryPercentage,
			HKQuantityTypeIdentifierWalkingStepLength,
			HKQuantityTypeIdentifierSixMinuteWalkTestDistance,
			HKQuantityTypeIdentifierStairAscentSpeed,
			HKQuantityTypeIdentifierStairDescentSpeed
		]];
	}
	if (@available(iOS 14.5, *)) {
		[quantityTypes addObject:HKQuantityTypeIdentifierAppleMoveTime];
	}
	
	// Vitals
	[quantityTypes addObjectsFromArray:@[
		HKQuantityTypeIdentifierHeartRate,
		HKQuantityTypeIdentifierBodyTemperature,
		HKQuantityTypeIdentifierBasalBodyTemperature,
		HKQuantityTypeIdentifierBloodPressureSystolic,
		HKQuantityTypeIdentifierBloodPressureDiastolic,
		HKQuantityTypeIdentifierRespiratoryRate
	]];
	
	if (@available(iOS 11.0, *)) {
		[quantityTypes addObjectsFromArray:@[
			HKQuantityTypeIdentifierRestingHeartRate,
			HKQuantityTypeIdentifierWalkingHeartRateAverage,
			HKQuantityTypeIdentifierHeartRateVariabilitySDNN
		]];
	}
	
	// Results
	[quantityTypes addObjectsFromArray:@[
		HKQuantityTypeIdentifierOxygenSaturation,
		HKQuantityTypeIdentifierPeripheralPerfusionIndex,
		HKQuantityTypeIdentifierBloodGlucose,
		HKQuantityTypeIdentifierNumberOfTimesFallen,
		HKQuantityTypeIdentifierElectrodermalActivity,
		HKQuantityTypeIdentifierInhalerUsage,
		// insulin
		HKQuantityTypeIdentifierBloodAlcoholContent,
		HKQuantityTypeIdentifierForcedVitalCapacity,
		HKQuantityTypeIdentifierForcedExpiratoryVolume1,
		HKQuantityTypeIdentifierPeakExpiratoryFlowRate
	]];
	if (@available(iOS 11.0, *)) {
		[quantityTypes addObject:HKQuantityTypeIdentifierInsulinDelivery];
	}
	if (@available(iOS 13.0, *)) {
		[quantityTypes addObjectsFromArray:@[
			HKQuantityTypeIdentifierEnvironmentalAudioExposure,
			HKQuantityTypeIdentifierHeadphoneAudioExposure
		]];
	}
	
	// Nutrition
	[quantityTypes addObjectsFromArray:@[
		HKQuantityTypeIdentifierDietaryFatTotal,
		HKQuantityTypeIdentifierDietaryFatPolyunsaturated,
		HKQuantityTypeIdentifierDietaryFatMonounsaturated,
		HKQuantityTypeIdentifierDietaryFatSaturated,
		HKQuantityTypeIdentifierDietaryCholesterol,
		HKQuantityTypeIdentifierDietarySodium,
		HKQuantityTypeIdentifierDietaryCarbohydrates,
		HKQuantityTypeIdentifierDietaryFiber,
		HKQuantityTypeIdentifierDietarySugar,
		HKQuantityTypeIdentifierDietaryEnergyConsumed,
		HKQuantityTypeIdentifierDietaryProtein,
		HKQuantityTypeIdentifierDietaryVitaminA,
		HKQuantityTypeIdentifierDietaryVitaminB6,
		HKQuantityTypeIdentifierDietaryVitaminB12,
		HKQuantityTypeIdentifierDietaryVitaminC,
		HKQuantityTypeIdentifierDietaryVitaminD,
		HKQuantityTypeIdentifierDietaryVitaminE,
		HKQuantityTypeIdentifierDietaryVitaminK,
		HKQuantityTypeIdentifierDietaryCalcium,
		HKQuantityTypeIdentifierDietaryIron,
		HKQuantityTypeIdentifierDietaryThiamin,
		HKQuantityTypeIdentifierDietaryRiboflavin,
		HKQuantityTypeIdentifierDietaryNiacin,
		HKQuantityTypeIdentifierDietaryFolate,
		HKQuantityTypeIdentifierDietaryBiotin,
		HKQuantityTypeIdentifierDietaryPantothenicAcid,
		HKQuantityTypeIdentifierDietaryPhosphorus,
		HKQuantityTypeIdentifierDietaryIodine,
		HKQuantityTypeIdentifierDietaryMagnesium,
		HKQuantityTypeIdentifierDietaryZinc,
		HKQuantityTypeIdentifierDietarySelenium,
		HKQuantityTypeIdentifierDietaryCopper,
		HKQuantityTypeIdentifierDietaryManganese,
		HKQuantityTypeIdentifierDietaryChromium,
		HKQuantityTypeIdentifierDietaryMolybdenum,
		HKQuantityTypeIdentifierDietaryChloride,
		HKQuantityTypeIdentifierDietaryPotassium,
		HKQuantityTypeIdentifierDietaryCaffeine,
		HKQuantityTypeIdentifierDietaryWater
	]];
	
	[quantityTypes addObject:HKQuantityTypeIdentifierUVExposure];
	
	if (@available(iOS 11.0, *)) {
		
	}
	
	return quantityTypes;
}


- (NSSet *)dataTypes:(NSArray *)staticIdentifiers writePermission:(BOOL)write
{
	// HKCharacteristicTypes, HKCategoryTypeIdentifierAppleStandHour, HKQuantityTypeIdentifierAppleExerciseTime disallowed
	
	NSMutableArray *identifiers = [staticIdentifiers mutableCopy];
	NSMutableSet *dataTypes = [NSMutableSet set];

	/*--------------------------------*/
	/*   HKQuantityType Identifiers   */
	/*--------------------------------*/

	void (^checkQuantityType)(NSString *, NSMutableArray *, NSMutableSet *) = ^void (NSString *identifier, NSMutableArray *allIdentifiers, NSMutableSet *dataTypes)
	{
		if ([allIdentifiers containsObject:identifier]) {
			NSLog(@"adding %@", identifier);
			[dataTypes addObject:[HKQuantityType quantityTypeForIdentifier:identifier]];
			[allIdentifiers removeObject:identifier];
		}

	};

	NSArray *quantityTypes = [self quantityTypes:write];
	
	for (NSString *identifier in quantityTypes) {
		checkQuantityType(identifier, identifiers, dataTypes);
//		NSLog(@"default unit for %@ = %@", identifier, [self defaultUnitForSampleType:[HKSampleType quantityTypeForIdentifier:identifier]]);
	}


	/*--------------------------------*/
	/*   HKCategoryType Identifiers   */
	/*--------------------------------*/

	void (^checkCategoryType)(NSString *, NSMutableArray *, NSMutableSet *) = ^void (NSString *identifier, NSMutableArray *allIdentifiers, NSMutableSet *dataTypes)
	{
		if ([allIdentifiers containsObject:identifier]) {
			[dataTypes addObject:[HKCategoryType categoryTypeForIdentifier:identifier]];
			[allIdentifiers removeObject:identifier];
		}

	};


	checkCategoryType(HKCategoryTypeIdentifierSleepAnalysis, identifiers, dataTypes);
	
	if (@available(iOS 9.0, *)) {
		if (!write) {
			checkCategoryType(HKCategoryTypeIdentifierAppleStandHour, identifiers, dataTypes);
		} else {
			[identifiers removeObject:HKCategoryTypeIdentifierAppleStandHour];
		}
	
		checkCategoryType(HKCategoryTypeIdentifierCervicalMucusQuality, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierOvulationTestResult, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierMenstrualFlow, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierIntermenstrualBleeding, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierSexualActivity, identifiers, dataTypes);
	}
	if (@available(iOS 10.0, *)) {
		checkCategoryType(HKCategoryTypeIdentifierMindfulSession, identifiers, dataTypes);
	}
	if (@available(iOS 12.2, *)) {
		checkCategoryType(HKCategoryTypeIdentifierHighHeartRateEvent, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierLowHeartRateEvent, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierIrregularHeartRhythmEvent, identifiers, dataTypes);
	}
	if (@available(iOS 13.0, *)) {
		checkCategoryType(HKCategoryTypeIdentifierAudioExposureEvent, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierToothbrushingEvent, identifiers, dataTypes);
	}
	
	if (@available(iOS 13.6, *)) {
		checkCategoryType(HKCategoryTypeIdentifierAbdominalCramps, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierAcne, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierAppetiteChanges, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierBloating, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierBreastPain, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierChestTightnessOrPain, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierChills, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierConstipation, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierCoughing, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierDiarrhea, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierDizziness, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierFainting, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierFatigue, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierFever, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierGeneralizedBodyAche, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierHeadache, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierHeartburn, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierHotFlashes, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierLossOfSmell, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierLossOfTaste, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierLowerBackPain, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierMoodChanges, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierNausea, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierPelvicPain, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierRapidPoundingOrFlutteringHeartbeat, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierRunnyNose, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierShortnessOfBreath, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierSinusCongestion, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierSkippedHeartbeat, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierSleepChanges, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierSoreThroat, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierVomiting, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierWheezing, identifiers, dataTypes);
	}
	if (@available(iOS 14.0, *)) {
		checkCategoryType(HKCategoryTypeIdentifierEnvironmentalAudioExposureEvent, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierHandwashingEvent, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierBladderIncontinence, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierDrySkin, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierHairLoss, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierMemoryLapse, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierNightSweats, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierVaginalDryness, identifiers, dataTypes);
	}
	if (@available(iOS 14.2, *)) { checkCategoryType(HKCategoryTypeIdentifierHeadphoneAudioExposureEvent, identifiers, dataTypes); }
	if (@available(iOS 14.3, *)) {
		checkCategoryType(HKCategoryTypeIdentifierPregnancy, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierLactation, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierContraceptive, identifiers, dataTypes);
		checkCategoryType(HKCategoryTypeIdentifierLowCardioFitnessEvent, identifiers, dataTypes);
	}

	/*--------------------------------------*/
	/*   HKCharacteristicType Identifiers   */
	/*--------------------------------------*/
	
	
	if (!write) {
		void (^checkCharacteristicType)(NSString *, NSMutableArray *, NSMutableSet *) = ^void (NSString *identifier, NSMutableArray *allIdentifiers, NSMutableSet *dataTypes)
		{
			if ([allIdentifiers containsObject:identifier]) {
				[dataTypes addObject:[HKCharacteristicType characteristicTypeForIdentifier:identifier]];
				[allIdentifiers removeObject:identifier];
			}

		};

		checkCharacteristicType(HKCharacteristicTypeIdentifierBiologicalSex, identifiers, dataTypes);
		checkCharacteristicType(HKCharacteristicTypeIdentifierBloodType, identifiers, dataTypes);
		checkCharacteristicType(HKCharacteristicTypeIdentifierDateOfBirth, identifiers, dataTypes);
		if (@available(iOS 9.0, *)) { checkCharacteristicType(HKCharacteristicTypeIdentifierFitzpatrickSkinType, identifiers, dataTypes); }
		if (@available(iOS 10.0, *)) { checkCharacteristicType(HKCharacteristicTypeIdentifierWheelchairUse, identifiers, dataTypes); }
		if (@available(iOS 14.0, *)) { checkCharacteristicType(HKCharacteristicTypeIdentifierActivityMoveMode, identifiers, dataTypes); }
	} else {
		[identifiers removeObjectsInArray:@[HKCharacteristicTypeIdentifierBiologicalSex, HKCharacteristicTypeIdentifierBloodType, HKCharacteristicTypeIdentifierDateOfBirth]];
		if (@available(iOS 9.0, *)) { [identifiers removeObject:HKCharacteristicTypeIdentifierFitzpatrickSkinType]; }
		if (@available(iOS 10.0, *)) { [identifiers removeObject:HKCharacteristicTypeIdentifierWheelchairUse]; }
	}

	/*-----------------------------------*/
	/*   HKCorrelationType Identifiers   */
	/*-----------------------------------*/

	void (^checkCorrelationType)(NSString *, NSMutableArray *, NSMutableSet *) = ^void (NSString *identifier, NSMutableArray *allIdentifiers, NSMutableSet *dataTypes)
	{
		void (^addQuantityIdentifier)(NSString *, NSMutableArray *, NSMutableSet *) = ^void (NSString *identifier, NSMutableArray *allIdentifiers, NSMutableSet *dataTypes)
		{
			[dataTypes addObject:[HKQuantityType quantityTypeForIdentifier:identifier]];
			[allIdentifiers removeObject:identifier];
		};

		if ([allIdentifiers containsObject:identifier]) {
			if ([identifier isEqualToString:HKCorrelationTypeIdentifierBloodPressure]) {
				addQuantityIdentifier(HKQuantityTypeIdentifierBloodPressureSystolic, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierBloodPressureDiastolic, allIdentifiers, dataTypes);
			}
			else if ([identifier isEqualToString:HKCorrelationTypeIdentifierFood]) {
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryEnergyConsumed, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryFatTotal, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryFatPolyunsaturated, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryFatMonounsaturated, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryFatSaturated, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryCholesterol, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietarySodium, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryCarbohydrates, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryFiber, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietarySugar, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryEnergyConsumed, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryProtein, allIdentifiers, dataTypes);

				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryVitaminA, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryVitaminB6, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryVitaminB12, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryVitaminC, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryVitaminD, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryVitaminE, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryVitaminK, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryCalcium, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryIron, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryThiamin, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryRiboflavin, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryNiacin, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryFolate, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryBiotin, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryPantothenicAcid, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryPhosphorus, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryIodine, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryMagnesium, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryZinc, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietarySelenium, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryCopper, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryManganese, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryChromium, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryMolybdenum, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryChloride, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryPotassium, allIdentifiers, dataTypes);
				addQuantityIdentifier(HKQuantityTypeIdentifierDietaryCaffeine, allIdentifiers, dataTypes);
			}
			[allIdentifiers removeObject:identifier];
		}

	};

	checkCorrelationType(HKCorrelationTypeIdentifierBloodPressure, identifiers, dataTypes);
	checkCorrelationType(HKCorrelationTypeIdentifierFood, identifiers, dataTypes);

	/*------------------------------*/
	/*   HKWorkoutType Identifier   */
	/*------------------------------*/

	void (^checkWorkoutType)(NSString *, NSMutableArray *, NSMutableSet *) = ^void (NSString *identifier, NSMutableArray *allIdentifiers, NSMutableSet *dataTypes)
	{
		if ([allIdentifiers containsObject:identifier]) {
			[dataTypes addObject:[HKWorkoutType workoutType]];
			[allIdentifiers removeObject:identifier];
		}

	};


	checkWorkoutType(HKWorkoutTypeIdentifier, identifiers, dataTypes);

	/*-------------------------------------*/
	/*   Document & Clinical Identifiers   */
	/*-------------------------------------*/

	void (^checkDocumentType)(NSString *, NSMutableArray *, NSMutableSet *) = ^void (NSString *identifier, NSMutableArray *allIdentifiers, NSMutableSet *dataTypes)
	{
		if ([allIdentifiers containsObject:identifier]) {
			[dataTypes addObject:[HKDocumentType documentTypeForIdentifier:identifier]];
			[allIdentifiers removeObject:identifier];
		}
		
	};

	
	checkDocumentType(HKDocumentTypeIdentifierCDA, identifiers, dataTypes);
	
	
	if (@available(iOS 12.0, *)) {
		void (^checkClinicalType)(NSString *, NSMutableArray *, NSMutableSet *) = ^void (NSString *identifier, NSMutableArray *allIdentifiers, NSMutableSet *dataTypes)
		{
			if ([allIdentifiers containsObject:identifier]) {
				[dataTypes addObject:[HKClinicalType clinicalTypeForIdentifier:identifier]];
				[allIdentifiers removeObject:identifier];
			}
			
		};

		checkClinicalType(HKClinicalTypeIdentifierAllergyRecord, identifiers, dataTypes);
		checkClinicalType(HKClinicalTypeIdentifierConditionRecord, identifiers, dataTypes);
		checkClinicalType(HKClinicalTypeIdentifierImmunizationRecord, identifiers, dataTypes);
		checkClinicalType(HKClinicalTypeIdentifierLabResultRecord, identifiers, dataTypes);
		checkClinicalType(HKClinicalTypeIdentifierMedicationRecord, identifiers, dataTypes);
		checkClinicalType(HKClinicalTypeIdentifierProcedureRecord, identifiers, dataTypes);
		checkClinicalType(HKClinicalTypeIdentifierVitalSignRecord, identifiers, dataTypes);
	}
	
	// ---------

	if (identifiers.count > 0) {
		NSLog(@"Error: failed to find data types for %@", identifiers);
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_TYPE_NOT_FOUND userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"Unable to find data types for identifiers:(%@)", identifiers]}];
		[self errorOccurred:err];
	}

	return dataTypes;
}

- (HKUnit *)defaultUnitForSampleType:(HKSampleType *)sampleType
{
	NSLocale *currentLocale = [NSLocale currentLocale];

	if ([sampleType isKindOfClass:[HKQuantityType class]]) {
		HKQuantityType *quantityType = (HKQuantityType *)sampleType;
		
		NSArray *countTypes = @[HKQuantityTypeIdentifierBodyMassIndex, HKQuantityTypeIdentifierStepCount, HKQuantityTypeIdentifierFlightsClimbed, HKQuantityTypeIdentifierNikeFuel, HKQuantityTypeIdentifierNumberOfTimesFallen, HKQuantityTypeIdentifierInhalerUsage, HKQuantityTypeIdentifierPushCount, HKQuantityTypeIdentifierSwimmingStrokeCount];
		if (@available(iOS 9.0, *)) {
			countTypes = [countTypes arrayByAddingObject:HKQuantityTypeIdentifierUVExposure];
		}
		if ([countTypes containsObject:quantityType.identifier]) {
			return [HKUnit countUnit];
		}
		
		if (@available(iOS 11.0, *)) {
			if ([quantityType.identifier isEqual:HKQuantityTypeIdentifierInsulinDelivery]) return [HKUnit internationalUnit];
		}
		
		NSArray *percentTypes = @[HKQuantityTypeIdentifierBodyFatPercentage, HKQuantityTypeIdentifierOxygenSaturation, HKQuantityTypeIdentifierPeripheralPerfusionIndex, HKQuantityTypeIdentifierBloodAlcoholContent, HKQuantityTypeIdentifierWalkingAsymmetryPercentage, HKQuantityTypeIdentifierWalkingDoubleSupportPercentage];
		if ([percentTypes containsObject:quantityType.identifier]) {
			return [HKUnit percentUnit];
		}
		
		if ([sampleType.identifier isEqualToString:HKQuantityTypeIdentifierHeight]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [HKUnit meterUnit];
			} else {
				return [HKUnit footUnit];
			}
		}
		if ([sampleType.identifier isEqualToString:HKQuantityTypeIdentifierWaistCircumference]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [HKUnit meterUnitWithMetricPrefix:HKMetricPrefixCenti];
			} else {
				return [HKUnit inchUnit];
			}
		}
		
		NSArray *distanceTypes = @[HKQuantityTypeIdentifierDistanceWalkingRunning, HKQuantityTypeIdentifierDistanceCycling, HKQuantityTypeIdentifierSixMinuteWalkTestDistance, HKQuantityTypeIdentifierWalkingStepLength, HKQuantityTypeIdentifierDistanceWheelchair, HKQuantityTypeIdentifierDistanceSwimming];
		if ([distanceTypes containsObject:quantityType.identifier]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [HKUnit meterUnitWithMetricPrefix:HKMetricPrefixKilo];
			} else {
				return [HKUnit mileUnit];
			}
		}
		
		NSArray *distanceTimeTypes = @[HKQuantityTypeIdentifierWalkingSpeed, HKQuantityTypeIdentifierStairAscentSpeed, HKQuantityTypeIdentifierStairDescentSpeed, HKQuantityTypeIdentifierDistanceDownhillSnowSports];
		if ([distanceTimeTypes containsObject:quantityType.identifier]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [[HKUnit meterUnit] unitDividedByUnit: [HKUnit minuteUnit]];
			} else {
				return [[HKUnit mileUnit] unitDividedByUnit: [HKUnit minuteUnit]];
			}
		}
		
		NSArray *weightTypes = @[HKQuantityTypeIdentifierBodyMass, HKQuantityTypeIdentifierLeanBodyMass];
		if ([weightTypes containsObject:quantityType.identifier]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [HKUnit gramUnitWithMetricPrefix:HKMetricPrefixKilo];
			} else {
				return [HKUnit poundUnit];
			}
		}
		
		NSArray *nutritionTypes = @[HKQuantityTypeIdentifierDietaryFatTotal, HKQuantityTypeIdentifierDietaryFatTotal, HKQuantityTypeIdentifierDietaryFatPolyunsaturated, HKQuantityTypeIdentifierDietaryFatMonounsaturated, HKQuantityTypeIdentifierDietaryFatSaturated,
									HKQuantityTypeIdentifierDietaryCholesterol, HKQuantityTypeIdentifierDietarySodium, HKQuantityTypeIdentifierDietaryCarbohydrates, HKQuantityTypeIdentifierDietaryFiber, HKQuantityTypeIdentifierDietarySugar, HKQuantityTypeIdentifierDietaryProtein,
									HKQuantityTypeIdentifierDietaryVitaminA, HKQuantityTypeIdentifierDietaryVitaminB6, HKQuantityTypeIdentifierDietaryVitaminB12, HKQuantityTypeIdentifierDietaryVitaminC, HKQuantityTypeIdentifierDietaryVitaminD, HKQuantityTypeIdentifierDietaryVitaminE,
									HKQuantityTypeIdentifierDietaryVitaminK, HKQuantityTypeIdentifierDietaryCalcium, HKQuantityTypeIdentifierDietaryIron, HKQuantityTypeIdentifierDietaryThiamin, HKQuantityTypeIdentifierDietaryRiboflavin, HKQuantityTypeIdentifierDietaryNiacin,
									HKQuantityTypeIdentifierDietaryFolate, HKQuantityTypeIdentifierDietaryBiotin, HKQuantityTypeIdentifierDietaryPantothenicAcid, HKQuantityTypeIdentifierDietaryPhosphorus, HKQuantityTypeIdentifierDietaryIodine, HKQuantityTypeIdentifierDietaryMagnesium,
									HKQuantityTypeIdentifierDietaryZinc, HKQuantityTypeIdentifierDietarySelenium, HKQuantityTypeIdentifierDietaryCopper, HKQuantityTypeIdentifierDietaryManganese, HKQuantityTypeIdentifierDietaryChromium, HKQuantityTypeIdentifierDietaryMolybdenum,
									HKQuantityTypeIdentifierDietaryChloride, HKQuantityTypeIdentifierDietaryPotassium, HKQuantityTypeIdentifierDietaryCaffeine];
		if ([nutritionTypes containsObject:quantityType.identifier]) {
			return [HKUnit gramUnit];
		}
		
		NSArray *calorieTypes = @[HKQuantityTypeIdentifierBasalEnergyBurned, HKQuantityTypeIdentifierActiveEnergyBurned, HKQuantityTypeIdentifierDietaryEnergyConsumed];
		if ([calorieTypes containsObject:quantityType.identifier]) {
			return [HKUnit kilocalorieUnit];
		}
		
		NSArray *tempTypes = @[HKQuantityTypeIdentifierBodyTemperature];
		if (@available(iOS 9.0, *)) { tempTypes = [tempTypes arrayByAddingObject:HKQuantityTypeIdentifierBasalBodyTemperature]; }
		if ([tempTypes containsObject:quantityType.identifier]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [HKUnit degreeCelsiusUnit];
			} else {
				return [HKUnit degreeFahrenheitUnit];
			}
		}
		
		NSArray *timeTypes = @[HKQuantityTypeIdentifierAppleExerciseTime, HKQuantityTypeIdentifierAppleStandTime, HKQuantityTypeIdentifierAppleMoveTime];
		if ([timeTypes containsObject:quantityType.identifier]) {
			return [HKUnit minuteUnit];
		}
		
		if (@available(iOS 11.0, *)) {
			timeTypes = @[HKQuantityTypeIdentifierHeartRateVariabilitySDNN];
			if ([timeTypes containsObject:quantityType.identifier]) {
				return [HKUnit secondUnitWithMetricPrefix:HKMetricPrefixMilli];
			}
		}
		
		NSArray *volumeUnits = @[HKQuantityTypeIdentifierForcedVitalCapacity, HKQuantityTypeIdentifierForcedExpiratoryVolume1];
		if (@available(iOS 9.0, *)) { volumeUnits = [volumeUnits arrayByAddingObject:HKQuantityTypeIdentifierDietaryWater]; }
		if ([volumeUnits containsObject:quantityType.identifier]) {
			return [HKUnit literUnit];
		}

		if (@available(iOS 13.0, *)) {
			NSArray *pressureUnits = @[HKQuantityTypeIdentifierEnvironmentalAudioExposure, HKQuantityTypeIdentifierHeadphoneAudioExposure];
			if ([pressureUnits containsObject:quantityType.identifier]) {
			return [HKUnit decibelAWeightedSoundPressureLevelUnit];
		}
		}


		NSArray *countTimeUnits = @[HKQuantityTypeIdentifierHeartRate, HKQuantityTypeIdentifierRestingHeartRate, HKQuantityTypeIdentifierWalkingHeartRateAverage];
		if ([countTimeUnits containsObject:quantityType.identifier]) {
			return [[HKUnit countUnit] unitDividedByUnit:[HKUnit minuteUnit]];
		}
		if ([quantityType.identifier isEqual:HKQuantityTypeIdentifierBloodPressureSystolic]) return [HKUnit millimeterOfMercuryUnit];
		if ([quantityType.identifier isEqual:HKQuantityTypeIdentifierBloodPressureDiastolic]) return [HKUnit millimeterOfMercuryUnit];
		if ([quantityType.identifier isEqual:HKQuantityTypeIdentifierRespiratoryRate]) return [[HKUnit countUnit] unitDividedByUnit:[HKUnit minuteUnit]];
		
		// Results
		if ([quantityType.identifier isEqual:HKQuantityTypeIdentifierBloodGlucose]) return [[HKUnit gramUnitWithMetricPrefix:HKMetricPrefixMilli] unitDividedByUnit:[HKUnit literUnitWithMetricPrefix:HKMetricPrefixDeci]];
		if ([quantityType.identifier isEqual:HKQuantityTypeIdentifierElectrodermalActivity]) return [HKUnit siemenUnitWithMetricPrefix:HKMetricPrefixMicro];

		if ([quantityType.identifier isEqual:HKQuantityTypeIdentifierPeakExpiratoryFlowRate]) return  [[HKUnit literUnit] unitDividedByUnit:[HKUnit minuteUnit]];
	
		if ([quantityType.identifier isEqual:HKQuantityTypeIdentifierVO2Max]) {
			// ml/(kg*min)
			return [[HKUnit literUnitWithMetricPrefix:HKMetricPrefixMilli] unitDividedByUnit:[[HKUnit gramUnitWithMetricPrefix:HKMetricPrefixKilo] unitMultipliedByUnit:[HKUnit minuteUnit]]];
		}
	}
	

	NSLog(@"Error; not sure what unit to use for %@", sampleType);
	NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_NO_DEFAULT_UNIT userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"No default unit for sample type %@.", sampleType]}];
	[self errorOccurred:err];
	return nil;
}

- (HKUnit *)genericUnitForSampleType:(HKSampleType *)sampleType
{
	NSLocale *currentLocale = [NSLocale currentLocale];

	if ([sampleType isKindOfClass:[HKQuantityType class]]) {
		HKQuantityType *quantityType = (HKQuantityType *)sampleType;

		HKUnit *unit = nil;

		// Scalar(Count)
		if ([quantityType isCompatibleWithUnit:[HKUnit countUnit]]) return [HKUnit countUnit];
		// Scalar(Percent, 0.0 - 1.0)
		if ([quantityType isCompatibleWithUnit:[HKUnit percentUnit]]) return [HKUnit percentUnit];
		// Energy
		if ([quantityType isCompatibleWithUnit:[HKUnit calorieUnit]]) return [HKUnit calorieUnit];
		// Length
		if ([quantityType isCompatibleWithUnit:[HKUnit meterUnit]]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [HKUnit meterUnit];
			} else {
				return [HKUnit footUnit];
			}
		}
		// Mass
		if ([quantityType isCompatibleWithUnit:[HKUnit poundUnit]]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [HKUnit gramUnitWithMetricPrefix:HKMetricPrefixKilo];
			} else {
				return [HKUnit poundUnit];
			}
		}
		// Temperature
		if ([quantityType isCompatibleWithUnit:[HKUnit degreeCelsiusUnit]]) {
			if ([[currentLocale objectForKey:NSLocaleUsesMetricSystem] boolValue]) {
				return [HKUnit degreeCelsiusUnit];
			} else {
				return [HKUnit degreeFahrenheitUnit];
			}
		}
		// Scalar(Count)/Time
		unit = [[HKUnit countUnit] unitDividedByUnit:[HKUnit minuteUnit]];
		if ([quantityType isCompatibleWithUnit:unit]) {
			return unit;
		}
		// Pressure
		if ([quantityType isCompatibleWithUnit:[HKUnit millimeterOfMercuryUnit]]) {
			return [HKUnit millimeterOfMercuryUnit];
		}
		// Mass/Volume
		unit = [[HKUnit gramUnitWithMetricPrefix:HKMetricPrefixMilli] unitDividedByUnit:[HKUnit literUnitWithMetricPrefix:HKMetricPrefixDeci]];
		if ([quantityType isCompatibleWithUnit:unit]) {
			return unit;
		}
		// Conductance
		if ([quantityType isCompatibleWithUnit:[HKUnit siemenUnit]]) {
			return [HKUnit siemenUnitWithMetricPrefix:HKMetricPrefixMicro];
		}
		// Volume
		if ([quantityType isCompatibleWithUnit:[HKUnit literUnit]]) {
			return [HKUnit literUnit];
		}
		// Volume/Time
		unit = [[HKUnit literUnit] unitDividedByUnit:[HKUnit hourUnit]];
		if ([quantityType isCompatibleWithUnit:unit]) {
			return unit;
		}
	}

	NSLog(@"Error; not sure what unit to use for %@", sampleType);
	NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_NO_DEFAULT_UNIT userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"No default unit for sample type %@.", sampleType]}];
	[self errorOccurred:err];
	return nil;
}

@end

// -------------------------------------
// MARK: - External interface
// -------------------------------------

void _InitializeNative(char *controllerName)
{
	//	[[BEHealthKit sharedHealthKit] start];
	[BEHealthKit sharedHealthKit].controllerName = [NSString stringWithCString:controllerName encoding:NSUTF8StringEncoding];
}

void _Authorize(char *dataTypesString)
{
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	NSArray *types = parseTransmission(dataTypesString);
	[kit authorizeHealthKitToRead:types[0] write:types[1] completion:^(bool success, NSError *error) {
		if (!success) {
			NSLog(@"Error authorizing healthkit:%@", error);
			[kit errorOccurred:error];
		}
		
		NSString *response = (success) ? @"true" : @"false";
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "AuthorizeComplete", [response cStringUsingEncoding:NSUTF8StringEncoding]);
	}];
}

int _AuthorizationStatusForType(char *dataTypeString)
{
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	return [kit authorizationStatusForType:[NSString stringWithCString:dataTypeString encoding:NSUTF8StringEncoding]];
}

BOOL _IsHealthDataAvailable()
{
	return [HKHealthStore isHealthDataAvailable];
}

NSArray *parseTransmission(char *dataTypesString) {
	NSArray *strings = [[NSString stringWithCString:dataTypesString encoding:NSUTF8StringEncoding] componentsSeparatedByString:@"|"];
	NSMutableArray *read = [[strings[0] componentsSeparatedByString:@","] mutableCopy];
	NSMutableArray *write = [[strings[1] componentsSeparatedByString:@","] mutableCopy];
	[read removeObject:@""];
	[write removeObject:@""];
	return @[read, write];
}
