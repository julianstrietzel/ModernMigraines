//
//  BEHealthKit+read.m
//  Unity-iPhone
//
//  Created by greay on 10/10/19.
//

#import "BEHealthKit+read.h"

#import "BEHealthKit.h"
#import "HealthData.h"

#import "NSDate+bridge.h"
#import "NSDateComponents+bridge.h"


@implementation BEHealthKit (read)

- (void)readSamples:(HKSampleType *)sampleType fromDate:(NSDate *)startDate toDate:(NSDate *)endDate resultsHandler:(void (^)(NSArray *results, NSError *error))resultsHandler
{
	NSDate *methodStart = [NSDate date];

	NSPredicate *predicate = [HKQuery predicateForSamplesWithStartDate:startDate endDate:endDate options:HKQueryOptionStrictStartDate];
	NSSortDescriptor *sortDescriptor = [NSSortDescriptor sortDescriptorWithKey:HKSampleSortIdentifierStartDate ascending:YES];

	HKSampleQuery *sampleQuery = [[HKSampleQuery alloc] initWithSampleType:sampleType
																 predicate:predicate
																	 limit:HKObjectQueryNoLimit
														   sortDescriptors:@[sortDescriptor]
															resultsHandler:^(HKSampleQuery *query, NSArray *results, NSError *error) {
																NSDate *methodFinish = [NSDate date];
																NSTimeInterval executionTime = [methodFinish timeIntervalSinceDate:methodStart];
																NSLog(@"--- querying HealthKit took %f seconds ---", executionTime);
																resultsHandler(results, error);
															}];

	[self.healthStore executeQuery:sampleQuery];

}

- (void)readSamplesForWorkoutActivity:(HKWorkoutActivityType)activity fromDate:(NSDate *)startDate toDate:(NSDate *)endDate resultsHandler:(void (^)(NSArray *results, NSError *error))resultsHandler
{
	NSDate *methodStart = [NSDate date];

	NSPredicate *activityPredicate = [HKQuery predicateForWorkoutsWithWorkoutActivityType:activity];
	NSPredicate *rangePredicate = [HKQuery predicateForSamplesWithStartDate:startDate endDate:endDate options:HKQueryOptionStrictStartDate];
	NSPredicate *predicate = [NSCompoundPredicate andPredicateWithSubpredicates:@[activityPredicate, rangePredicate]];

	NSSortDescriptor *sortDescriptor = [NSSortDescriptor sortDescriptorWithKey:HKSampleSortIdentifierStartDate ascending:YES];

	HKSampleQuery *sampleQuery = [[HKSampleQuery alloc] initWithSampleType:[HKWorkoutType workoutType]
																 predicate:predicate
																	 limit:HKObjectQueryNoLimit
														   sortDescriptors:@[sortDescriptor]
															resultsHandler:^(HKSampleQuery *query, NSArray *results, NSError *error) {
																NSDate *methodFinish = [NSDate date];
																NSTimeInterval executionTime = [methodFinish timeIntervalSinceDate:methodStart];
																NSLog(@"--- querying HealthKit took %f seconds ---", executionTime);
																resultsHandler(results, error);
															}];

	[self.healthStore executeQuery:sampleQuery];

}
- (void)readCharacteristic:(HKCharacteristicType *)characteristic resultsHandler:(void (^)(id result, NSError *error))resultsHandler
{
	NSDate *methodStart = [NSDate date];

	id result = nil;
	NSError *error = nil;

	if ([characteristic.identifier isEqualToString:HKCharacteristicTypeIdentifierBiologicalSex]) {
		result = [self.healthStore biologicalSexWithError:&error];
	} else if ([characteristic.identifier isEqualToString:HKCharacteristicTypeIdentifierBloodType]) {
		result = [self.healthStore bloodTypeWithError:&error];
	} else if ([characteristic.identifier isEqualToString:HKCharacteristicTypeIdentifierDateOfBirth]) {
		result = [self.healthStore dateOfBirthWithError:&error];
	}
	
	if (@available(iOS 9.0, *)) {
		if (!result && [characteristic.identifier isEqualToString:HKCharacteristicTypeIdentifierFitzpatrickSkinType]) {
			result = [self.healthStore fitzpatrickSkinTypeWithError:&error];
		}
	}
	if (@available(iOS 10.0, *)) {
		if (!result && [characteristic.identifier isEqualToString:HKCharacteristicTypeIdentifierWheelchairUse]) {
			result = [self.healthStore wheelchairUseWithError:&error];
		}
	}
	
	if (!result) {
		NSLog(@"error: unknown characteristic %@", characteristic);
	}

	NSDate *methodFinish = [NSDate date];
	NSTimeInterval executionTime = [methodFinish timeIntervalSinceDate:methodStart];
	NSLog(@"--- querying HealthKit took %f seconds ---", executionTime);
	resultsHandler(result, error);
}

- (void)readStatisticsForQuantityType:(HKQuantityType *)quantityType predicate:(NSPredicate *)predicate options:(HKStatisticsOptions)options resultsHandler:(void (^)(id result, NSError *error))resultsHandler
{
	HKStatisticsQuery *query = [[HKStatisticsQuery alloc] initWithQuantityType:quantityType quantitySamplePredicate:predicate options:options completionHandler:^(HKStatisticsQuery *query, HKStatistics *result, NSError *error) {
		resultsHandler(result, error);
	}];
	[self.healthStore executeQuery:query];

}


- (void)readStatisticsCollectionForQuantityType:(HKQuantityType *)quantityType predicate:(NSPredicate *)predicate options:(HKStatisticsOptions)options anchorDate:(NSDate *)anchorDate intervalComponents:(NSDateComponents *)interval resultsHandler:(void (^)(id result, NSError *error))resultsHandler
{
	HKStatisticsCollectionQuery *query = [[HKStatisticsCollectionQuery alloc] initWithQuantityType:quantityType quantitySamplePredicate:predicate options:options anchorDate:anchorDate intervalComponents:interval];
	query.initialResultsHandler = ^(HKStatisticsCollectionQuery *query, HKStatisticsCollection *result, NSError *error) {
		resultsHandler(result, error);
	};
	[self.healthStore executeQuery:query];
	
}

- (void)readDocumentOfType:(HKDocumentType *)documentType predicate:(NSPredicate *)predicate limit:(NSUInteger)limit sortDescriptors:(NSArray<NSSortDescriptor *> *)sortDescriptors includeDocumentData:(BOOL)includeDocumentData resultsHandler:(void (^)(id result, BOOL done, NSError *error))resultsHandler
{
	HKDocumentQuery * query = [[HKDocumentQuery alloc] initWithDocumentType:documentType predicate:predicate limit:limit sortDescriptors:sortDescriptors includeDocumentData:includeDocumentData resultsHandler:^(HKDocumentQuery * _Nonnull query, NSArray<__kindof HKDocumentSample *> * _Nullable results, BOOL done, NSError * _Nullable error) {
		resultsHandler(results, done, error);
	}];
	[self.healthStore executeQuery:query];
}

@end

// -------------------------------------
// MARK: Reading
// -------------------------------------

HKStatisticsOptions optionsForIdentifier(NSString *identifier);
void ReadQuantity(HKQuantityType *sampleType, NSDate *startDate, NSDate *endDate, bool combineSamples, bool latestOnly);
void BeginObserverQuery(HKSampleType *sampleType, void (^handler)(NSError *error));


void _ReadQuantity(char *identifier, char *startDateString, char *endDateString, bool combineSamples)
{
	NSString *identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
	HKQuantityType *sampleType = [HKSampleType quantityTypeForIdentifier:identifierString];
	if (!sampleType) {
		BEHealthKit *kit = [BEHealthKit sharedHealthKit];
		NSLog(@"Error; unknown quantity-type identifier '%@'", identifierString);
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_UNKNOWN_DATATYPE userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"Unknown quantity-type identifier %@.", identifierString]}];
		[kit errorOccurred:err];
		return;
	}
	
	NSDate *startDate = [NSDate dateFromBridgeString:startDateString];
	NSDate *endDate = [NSDate dateFromBridgeString:endDateString];
	
	ReadQuantity(sampleType, startDate, endDate, combineSamples, false);
}


void _ReadCategory(char *identifier, char *startDateString, char *endDateString)
{
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	
	NSString *identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
	HKCategoryType *sampleType = [HKSampleType categoryTypeForIdentifier:identifierString];
	if (!sampleType) {
		NSLog(@"Error; unknown category-type identifier '%@'", identifierString);
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_UNKNOWN_DATATYPE userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"Unknown category-type identifier %@.", identifierString]}];
		[kit errorOccurred:err];
		return;
	}
	
	NSDate *startDate = [NSDate dateFromBridgeString:startDateString];
	NSDate *endDate = [NSDate dateFromBridgeString:endDateString];
	NSLog(@"[reading category data from %@ to %@]", startDate, endDate);
	
	[kit readSamples:sampleType fromDate:startDate toDate:endDate resultsHandler:^(NSArray *results, NSError *error) {
		NSString *xml = nil;
		xml = [HealthData XMLFromCategorySamples:results datatype:identifierString error:error];
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
	}];
}

void _ReadCharacteristic(char *identifier)
{
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	
	NSString *identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
	HKCharacteristicType *characteristic = [HKCharacteristicType characteristicTypeForIdentifier:identifierString];
	if (!characteristic) {
		NSLog(@"Error; unknown characteristic-type identifier '%@'", identifierString);
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_UNKNOWN_DATATYPE userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"Unknown characteristic-type identifier %@.", identifierString]}];
		[kit errorOccurred:err];
		return;
	}
	
	[kit readCharacteristic:characteristic resultsHandler:^(id result, NSError *error) {
		NSString *xml = nil;
		xml = [HealthData XMLFromCharacteristic:result datatype:identifierString error:error];
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
	}];
}

void _ReadCorrelation(char *identifier, char *startDateString, char *endDateString, bool combineSamples)
{
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	
	NSString *identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
	HKCorrelationType *sampleType = [HKCorrelationType correlationTypeForIdentifier:identifierString];
	if (!sampleType) {
		NSLog(@"Error; unknown correlation-type identifier '%@'", identifierString);
		[kit errorOccurred:nil];
		return;
	}
	
	NSDate *startDate = [NSDate dateFromBridgeString:startDateString];
	NSDate *endDate = [NSDate dateFromBridgeString:endDateString];
	//	NSLog(@"[reading correlation from %@ to %@]", startDate, endDate);
	
	[kit readSamples:sampleType fromDate:startDate toDate:endDate resultsHandler:^(NSArray *results, NSError *error) {
		NSString *xml = nil;
		xml = [HealthData XMLFromCorrelationSamples:results datatype:identifierString error:error];
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
	}];
}

void _ReadWorkout(int activityID, char *startDateString, char *endDateString, bool combineSamples)
{
	HKWorkoutActivityType activityType = (HKWorkoutActivityType)activityID;
	
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	NSDate *startDate = [NSDate dateFromBridgeString:startDateString];
	NSDate *endDate = [NSDate dateFromBridgeString:endDateString];
	//	NSLog(@"[reading workout from %@ to %@]", startDate, endDate);
	
	[kit readSamplesForWorkoutActivity:activityType fromDate:startDate toDate:endDate resultsHandler:^(NSArray *results, NSError *error) {
		NSString *xml = nil;
		xml = [HealthData XMLFromWorkoutSamples:results workoutType:activityType error:error];
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
	}];
}

void _BeginObserverQuery(char *identifier)
{
	NSString *identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
	HKSampleType *sampleType = nil;
	if ([identifierString isEqualToString:HKWorkoutTypeIdentifier]) {
		sampleType = [HKSampleType workoutType];
	} else {
		sampleType = [HKSampleType quantityTypeForIdentifier:identifierString];
	}
	
	if (!sampleType) {
		NSLog(@"Error; unknown type identifier '%@'", identifierString);
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_UNKNOWN_DATATYPE userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"Unknown quantity-type identifier %@.", identifierString]}];
		[[BEHealthKit sharedHealthKit] errorOccurred:err];
		return;
	}
	
	__block HKQueryAnchor *anchor = nil;
	BeginObserverQuery(sampleType, ^(NSError *error) {
		if (error) {
			[[BEHealthKit sharedHealthKit] errorOccurred:error];
		} else {
			NSDate *startDate = [NSDate date];
			HKAnchoredObjectQuery *anchoredQuery = [[HKAnchoredObjectQuery alloc] initWithType:sampleType predicate:nil anchor:anchor limit:HKObjectQueryNoLimit resultsHandler:^(HKAnchoredObjectQuery *query, NSArray *samples, NSArray *deletedObjects, HKQueryAnchor *newAnchor, NSError *error) {
				
				if (anchor == nil) {
					// the first result will be EVERYTHING that matches the type; we only want new samples after the anchor is set.
					NSLog(@"seeded observer query results");
				} else {
					NSString *xml = [HealthData XMLFromSamples:samples datatype:sampleType error:error];
					UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
				}
				anchor = newAnchor;
			}];
			
			[[BEHealthKit sharedHealthKit].healthStore executeQuery:anchoredQuery];
		}
	});
}

void _ReadCombinedQuantityStatistics(char *identifier, char *startDateString, char *endDateString) {
	NSString *identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
	HKQuantityType *sampleType = [HKSampleType quantityTypeForIdentifier:identifierString];
	if (!sampleType) {
		NSLog(@"Error; unknown quantity-type identifier '%@'", identifierString);
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_UNKNOWN_DATATYPE userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"Unknown quantity-type identifier %@.", identifierString]}];
		[[BEHealthKit sharedHealthKit] errorOccurred:err];
		return;
	}

	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	NSDate *startDate = [NSDate dateFromBridgeString:startDateString];
	NSDate *endDate = [NSDate dateFromBridgeString:endDateString];
	NSLog(@"[reading quantity from %@ to %@]", startDate, endDate);
	

	NSPredicate *predicate = [HKQuery predicateForSamplesWithStartDate:startDate endDate:endDate options:HKQueryOptionStrictStartDate];
	[kit readStatisticsForQuantityType:sampleType predicate:predicate options:HKStatisticsOptionCumulativeSum resultsHandler:^(id result, NSError *error) {
		NSString *xml = [HealthData XMLFromStatistics:result datatype:sampleType error:error];
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
	}];
}

void _ReadStatistics(char *identifier, char *startDateString, char *endDateString, char *optionsString) {
	NSString *identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
	HKQuantityType *sampleType = [HKSampleType quantityTypeForIdentifier:identifierString];
	if (!sampleType) {
		NSLog(@"Error; unknown quantity-type identifier '%@'", identifierString);
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_UNKNOWN_DATATYPE userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"Unknown quantity-type identifier %@.", identifierString]}];
		[[BEHealthKit sharedHealthKit] errorOccurred:err];
		return;
	}
	
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	NSDate *startDate = [NSDate dateFromBridgeString:startDateString];
	NSDate *endDate = [NSDate dateFromBridgeString:endDateString];
	NSLog(@"[reading statistics from %@ to %@]", startDate, endDate);
	
	HKStatisticsOptions options = optionsForIdentifier([NSString stringWithCString:optionsString encoding:NSUTF8StringEncoding]);
	
	NSPredicate *predicate = [HKQuery predicateForSamplesWithStartDate:startDate endDate:endDate options:HKQueryOptionStrictStartDate];
	[kit readStatisticsForQuantityType:sampleType predicate:predicate options:options resultsHandler:^(id result, NSError *error) {
		NSString *xml = [HealthData XMLFromStatistics:result datatype:sampleType error:error];
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
	}];
}

void _ReadStatisticsCollection(char *identifier, char *predicateString, char *optionsString, char *anchorStamp, char *intervalString) {
	NSString *identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
	HKQuantityType *sampleType = [HKSampleType quantityTypeForIdentifier:identifierString];
	if (!sampleType) {
		NSLog(@"Error; unknown quantity-type identifier '%@'", identifierString);
		NSError *err = [NSError errorWithDomain:@"beliefengine" code:BEHK_ERROR_UNKNOWN_DATATYPE userInfo:@{NSLocalizedDescriptionKey:[NSString stringWithFormat:@"Unknown quantity-type identifier %@.", identifierString]}];
		[[BEHealthKit sharedHealthKit] errorOccurred:err];
		return;
	}
	
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	NSDate *anchorDate = [NSDate dateFromBridgeString:anchorStamp];
	HKStatisticsOptions options = optionsForIdentifier([NSString stringWithCString:optionsString encoding:NSUTF8StringEncoding]);
	NSPredicate *predicate = nil;
	NSDateComponents *interval = [NSDateComponents dateComponentsFromBridgeString:intervalString];

	NSLog(@"[reading statistics collection anchored to %@]", anchorDate);
	[kit readStatisticsCollectionForQuantityType:sampleType predicate:predicate options:options anchorDate:anchorDate intervalComponents:interval resultsHandler:^(id result, NSError *error) {
		NSString *xml = [HealthData XMLFromStatisticsCollection:result datatype:sampleType error:error];
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
	}];

}

void _ReadDocument(/* char *documentTypeString, */ char *predicateString, int limit, char *sort, bool includeData) {
	// THIS IS THE ONLY TYPE OF DOCUMENT CURRENTLY AVAILABLE!
	HKDocumentType *documentType = [HKDocumentType documentTypeForIdentifier:HKDocumentTypeIdentifierCDA];
	
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	NSPredicate *predicate = nil;
	__block NSMutableArray *totalResults = [NSMutableArray array];
	[kit readDocumentOfType:documentType predicate:predicate limit:(NSUInteger)limit sortDescriptors:nil includeDocumentData:includeData resultsHandler:^(id results, BOOL done, NSError *error) {
		NSLog(@"adding %lu results", (unsigned long)[results count]);
		[totalResults addObjectsFromArray:results];
		
		if (done) {
			NSString *xml = [HealthData XMLFromHealthDocuments:totalResults error:error];
			UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
		}
	}];
}


// -------------------------------------------------
// MARK: Pedometer
// -------------------------------------------------


void _ReadPedometer(char *startDateString, char *endDateString)
{
	NSDate *startDate = [NSDate dateFromBridgeString:startDateString];
	NSDate *endDate =   [NSDate dateFromBridgeString:endDateString];
	
	if (![BEHealthKit sharedHealthKit].pedometer) {
		[BEHealthKit sharedHealthKit].pedometer = [[BEPedometer alloc] init];
	}
	[[BEHealthKit sharedHealthKit].pedometer queryPedometerDataFromDate:startDate toDate:endDate];
}

void _StartReadingPedometerFromDate(char *startDateString)
{
	NSDate *startDate = [NSDate dateFromBridgeString:startDateString];
	
	if (![BEHealthKit sharedHealthKit].pedometer) {
		[BEHealthKit sharedHealthKit].pedometer = [[BEPedometer alloc] init];
	}
	[[BEHealthKit sharedHealthKit].pedometer startPedometerUpdatesFromDate:startDate];
}

void _StopReadingPedometer()
{
	[[BEHealthKit sharedHealthKit].pedometer stopPedometerUpdates];
}

// -------------------------------------------------
// MARK: - Internal
// -------------------------------------------------

void ReadQuantity(HKQuantityType *sampleType, NSDate *startDate, NSDate *endDate, bool combineSamples, bool latestOnly) {
	BEHealthKit *kit = [BEHealthKit sharedHealthKit];
	NSLog(@"[reading quantity from %@ to %@]", startDate, endDate);
	
	[kit readSamples:sampleType fromDate:startDate toDate:endDate resultsHandler:^(NSArray *results, NSError *error) {
		NSString *xml = nil;
		if (!combineSamples) {
			xml = [HealthData XMLFromQuantitySamples:results datatype:sampleType.identifier error:error];
		} else {
			HKUnit *unit = [kit defaultUnitForSampleType:sampleType];
			double total = 0;
			for (HKQuantitySample *sample in results) {
				total += [sample.quantity doubleValueForUnit:unit];
			}
			if (sampleType.aggregationStyle == HKQuantityAggregationStyleCumulative) {
				xml = [HealthData XMLFromCombinedTotal:total datatype:sampleType.identifier error:error];
			} else {
				double average = total / (double)results.count;
				xml = [HealthData XMLFromCombinedTotal:average datatype:sampleType.identifier error:error];
			}
		}
		UnitySendMessage([[BEHealthKit sharedHealthKit].controllerName cStringUsingEncoding:NSUTF8StringEncoding], "ParseHealthXML", [xml cStringUsingEncoding:NSUTF8StringEncoding]);
	}];
}

void BeginObserverQuery(HKSampleType *sampleType, void (^handler)(NSError *error)) {
	HKObserverQuery *query = [[HKObserverQuery alloc] initWithSampleType:sampleType predicate:nil updateHandler:^(HKObserverQuery *query, HKObserverQueryCompletionHandler completionHandler, NSError *error) {
		// Take whatever steps are necessary to update your app's data and UI
		// This may involve executing other queries
		handler(error);
		
		// If you have subscribed for background updates you must call the completion handler here.
		// completionHandler();
	}];
	
	[[BEHealthKit sharedHealthKit].healthStore executeQuery:query];
}

HKStatisticsOptions optionsForIdentifier(NSString *identifier) {
	/*
	 HKStatisticsOptionNone              		= 0,
	 HKStatisticsOptionSeparateBySource          = 1 << 0,
	 HKStatisticsOptionDiscreteAverage           = 1 << 1,
	 HKStatisticsOptionDiscreteMin               = 1 << 2,
	 HKStatisticsOptionDiscreteMax               = 1 << 3,
	 HKStatisticsOptionCumulativeSum             = 1 << 4,
	 HKStatisticsOptionDiscreteMostRecent API_AVAILABLE(ios(12.0), watchos(5.0))  = 1 << 5,
	 */
	if (!identifier || [identifier isEqualToString:@"None"]) return HKStatisticsOptionNone;
	if ([identifier isEqualToString:@"SeparateBySource"]) return HKStatisticsOptionSeparateBySource;
	if ([identifier isEqualToString:@"DiscreteAverage"]) return HKStatisticsOptionDiscreteAverage;
	if ([identifier isEqualToString:@"DiscreteMin"]) return HKStatisticsOptionDiscreteMin;
	if ([identifier isEqualToString:@"DiscreteMax"]) return HKStatisticsOptionDiscreteMax;
	if ([identifier isEqualToString:@"CumulativeSum"]) return HKStatisticsOptionCumulativeSum;
	if (@available(iOS 12.0, *)) {
		if ([identifier isEqualToString:@"DiscreteMostRecent"]) return HKStatisticsOptionDiscreteMostRecent;
	}
	
	return HKStatisticsOptionNone;
}
