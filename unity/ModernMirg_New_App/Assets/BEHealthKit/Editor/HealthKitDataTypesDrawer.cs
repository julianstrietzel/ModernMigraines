using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

namespace BeliefEngine.HealthKit
{

	/*! @brief Property drawer for HealthKitDataTypes */
	[CustomEditor (typeof (HealthKitDataTypes))]
	public partial class HealthKitDataTypesEditor : Editor
	{
		private HealthKitDataTypes obj;

		private bool bodyMeasurementSection = true;
		private bool fitnessSection = true;
		private bool vitalsSection = true;
		private bool resultsSection = true;
		private bool nutritionSection = true;
		private bool mobilitySection = true;

		private bool categorySection = true;
		private bool characteristicSection = true;
		private bool correlationSection = true;
		private bool otherSection = true;

		void Awake() {
			obj = (HealthKitDataTypes)target;
		}


		private SerializedProperty saveDataProperty;
		private SerializedProperty usageStringProperty;
		private SerializedProperty updateStringProperty;
		private SerializedProperty clinicalUsageStringProperty;
		void OnEnable() {
			this.saveDataProperty = serializedObject.FindProperty("saveData");
			this.usageStringProperty = serializedObject.FindProperty("healthShareUsageDescription");
			this.updateStringProperty = serializedObject.FindProperty("healthUpdateUsageDescription");
			this.clinicalUsageStringProperty = serializedObject.FindProperty("clinicalUsageDescription");
		}

		/*! @brief draws the GUI */
		public override void OnInspectorGUI() {
			serializedObject.Update();
		
			GUILayout.BeginVertical();

			EditorGUILayout.PropertyField(usageStringProperty, new GUIContent("Health Share Usage"), null);
			if (this.NeedsWriteAccess()) {
				EditorGUILayout.PropertyField(updateStringProperty, new GUIContent("Health Update Usage"), null);
			}
			if (this.NeedsClinicalAccess()) {
				EditorGUILayout.PropertyField(clinicalUsageStringProperty, new GUIContent("Clincal Data Usage"), null);
			}

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Datatype", EditorStyles.boldLabel, GUILayout.MaxWidth(240));
			EditorGUILayout.LabelField("read", EditorStyles.boldLabel, GUILayout.MaxWidth(40));
			EditorGUILayout.LabelField("write", EditorStyles.boldLabel, GUILayout.MaxWidth(40));
			GUILayout.EndHorizontal();

			bodyMeasurementSection = EditorGUILayout.Foldout(bodyMeasurementSection, "Body Measurements");
			if (bodyMeasurementSection) {
				DrawDataTypes(HKDataType.HKQuantityTypeIdentifierBodyMassIndex, HKDataType.HKQuantityTypeIdentifierWaistCircumference);
			}
			fitnessSection = EditorGUILayout.Foldout(fitnessSection, "Fitness");
			if (fitnessSection) {
				DrawDataTypes(HKDataType.HKQuantityTypeIdentifierStepCount, HKDataType.HKQuantityTypeIdentifierDistanceDownhillSnowSports);
			}
			vitalsSection = EditorGUILayout.Foldout(vitalsSection, "Vitals");
			if (vitalsSection) {
				DrawDataTypes(HKDataType.HKQuantityTypeIdentifierHeartRate, HKDataType.HKQuantityTypeIdentifierHeartRateVariabilitySDNN);
			}
			resultsSection = EditorGUILayout.Foldout(resultsSection, "Results");
			if (resultsSection) {
				DrawDataTypes(HKDataType.HKQuantityTypeIdentifierOxygenSaturation, HKDataType.HKQuantityTypeIdentifierHeadphoneAudioExposure);
			}
			nutritionSection = EditorGUILayout.Foldout(nutritionSection, "Nutrition");
			if (nutritionSection) {
				DrawDataTypes(HKDataType.HKQuantityTypeIdentifierDietaryFatTotal, HKDataType.HKQuantityTypeIdentifierDietaryWater);
			}
			mobilitySection = EditorGUILayout.Foldout(mobilitySection, "Mobility");
			if (mobilitySection) {
				DrawDataTypes(HKDataType.HKQuantityTypeIdentifierSixMinuteWalkTestDistance, HKDataType.HKQuantityTypeIdentifierStairDescentSpeed);
			}

			// categories

			categorySection = EditorGUILayout.Foldout(categorySection, "Categories");
			if (categorySection) {
				DrawDataTypes(HKDataType.HKCategoryTypeIdentifierSleepAnalysis, HKDataType.HKCategoryTypeIdentifierLowCardioFitnessEvent);
			}
			categorySection = EditorGUILayout.Foldout(categorySection, "Symptoms");
			if (categorySection) {
				DrawDataTypes(HKDataType.HKCategoryTypeIdentifierAbdominalCramps, HKDataType.HKCategoryTypeIdentifierWheezing);
			}

			characteristicSection = EditorGUILayout.Foldout(characteristicSection, "Characteristics");
			if (characteristicSection) {
				DrawDataTypes(HKDataType.HKCharacteristicTypeIdentifierBiologicalSex, HKDataType.HKCharacteristicTypeIdentifierActivityMoveMode);
			}
			correlationSection = EditorGUILayout.Foldout(correlationSection, "Correlations");
			if (correlationSection) {
				DrawDataTypes(HKDataType.HKCorrelationTypeIdentifierBloodPressure, HKDataType.HKCorrelationTypeIdentifierFood);
			}
			otherSection = EditorGUILayout.Foldout(otherSection, "Other");
			if (otherSection) {
				DrawDataType(HKDataType.HKQuantityTypeIdentifierUVExposure);
				DrawDataType(HKDataType.HKWorkoutTypeIdentifier);
				DrawDataType(HKDataType.HKDocumentTypeIdentifierCDA);
			}

			// this will do nothing if the clinical data extension isn't included
			this.DrawClinicalSupport();

			GUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawDataTypes<T>(T startType, T endType) where T : System.Enum {
			if (!typeof(T).IsEnum) {
				throw new ArgumentException("T must be an enumerated type");
			}
			for (int i = Convert.ToInt32(startType); i <= Convert.ToInt32(endType); i++) {
				DrawDataType( (T) (object) i );
			}
		}
		
		private void DrawDataType<T>(T dataType) where T: Enum {
			GUILayout.BeginHorizontal();
			Dictionary<string, HKNameValuePair> data = obj.data;
			if (data != null) {
				string key = HealthKitDataTypes.GetIdentifier<T>(dataType);
				if (data.ContainsKey(key)) {
					EditorGUILayout.LabelField(data[key].name, GUILayout.MaxWidth(240));

					EditorGUI.BeginChangeCheck();
					bool readValue = EditorGUILayout.Toggle(data[key].read, GUILayout.MaxWidth(40));
					if (EditorGUI.EndChangeCheck()) {
						data[key].read = readValue;
						string saveData = obj.Save();
						this.saveDataProperty.stringValue = saveData;
					}

					if (!data[key].writable) GUI.enabled = false;

					EditorGUI.BeginChangeCheck();
					bool writeValue = EditorGUILayout.Toggle(data[key].write, GUILayout.MaxWidth(40));
					if (EditorGUI.EndChangeCheck()) {
						data[key].write = writeValue;
						// EditorUtility.SetDirty(prop.serializedObject.targetObject);
						string saveData = obj.Save();
						this.saveDataProperty.stringValue = saveData;
					}

					GUI.enabled = true;
				} else {
					EditorGUILayout.LabelField(key, GUILayout.MaxWidth(240));
					EditorGUILayout.LabelField("ERROR", GUILayout.MaxWidth(80));
				}
			}
			GUILayout.EndHorizontal();
		}

		private bool NeedsWriteAccess() {
			return obj.AskForUpdatePermission();
		}

		private bool NeedsClinicalAccess() {
			return obj.AskForClinicalPermission();
		}


		partial void DrawClinicalSupport();
	}

}