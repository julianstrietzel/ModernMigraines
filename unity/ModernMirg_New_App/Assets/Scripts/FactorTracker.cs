using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine;
using Unity.Mathematics;
using System;
using Firebase.Database;

public class FactorTracker : MonoBehaviour
{
    //Personal and General Risk both range from 0 to 100.

    private int PersonalRisk;    // PersonalRisk = -1 if not enough information is available yet
    private int generalRisk;


    public int getPersonalRisk()
    {
        //Returns -1 if not enough info yet
        return PersonalRisk;
    }

    public int getGeneralRisk()
    {
        return generalRisk;
    }

    // Updates the variables PersonalRisk and generalRisk
    public void updateRisks()
    {
        float averageTemp = 0;
        float averagePressure = 0;
        float averageHumidity = 0;
        float averageTempChange = 0;
        float averagePressureChange = 0;
        float averageHumidityChange = 0;

        float mAverageTemp = 0;
        float mAveragePressure = 0;
        float mAverageHumidity = 0;
        float mAverageTempChange = 0;
        float mAveragePressureChange = 0;
        float mAverageHumidityChange = 0;

        float prevTemp = 0;
        float prevPressure = 0;
        float prevHumidity = 0;
        int dayCount = 0;
        int mDayCount = 0;

    //Iterate over all DayData objects
        foreach (KeyValuePair<int, DayData> day in LocalDataManager.dayDatas)
        {

            dayCount++;
            averageTemp += day.Value.temp_max;
            averagePressure += day.Value.pressure;
            averageHumidity += day.Value.humidity;

            if (dayCount <= 1)
            {
                averageTempChange += Math.Abs(day.Value.temp_max - prevTemp);
                averagePressureChange += Math.Abs(day.Value.pressure - prevPressure);
                averageHumidityChange += Math.Abs(day.Value.humidity - prevHumidity);
            }

            //Update migraine day values
            if (day.Value.migraine)
            {
                mDayCount++;
                mAverageTemp += day.Value.temp_max;
                mAveragePressure += day.Value.pressure;
                mAverageHumidity += day.Value.humidity;

                if (dayCount != 1)
                {
                    mAverageTempChange += Math.Abs(day.Value.temp_max - prevTemp);
                    mAveragePressureChange += Math.Abs(day.Value.pressure - prevPressure);
                    mAverageHumidityChange += Math.Abs(day.Value.humidity - prevHumidity);
                }

            }


            prevTemp = day.Value.temp_max;
            prevPressure = day.Value.pressure;
            prevHumidity = day.Value.humidity;

        }

        if(dayCount == 0){
            PersonalRisk = -1;
            generalRisk = -1;
            return;
        }

        if (dayCount != 0) {
            averageTemp /= dayCount;
            averagePressure /= dayCount;
            averageHumidity /= dayCount;
            averageTempChange /= dayCount;
            averagePressureChange /= dayCount;
            averageHumidityChange /= dayCount;
        }

        if (mDayCount != 0) {
            mAverageTemp /= mDayCount;
            mAveragePressure /= mDayCount;
            mAverageHumidity /= mDayCount;
            mAverageTempChange /= mDayCount;
            mAveragePressureChange /= mDayCount;
            mAverageHumidityChange /= mDayCount;
        }


        int days = dayCount;
        int migraineDays = mDayCount;


        //Current Day:

        DayData today = LocalDataManager.GetToday();
        DayData yest = LocalDataManager.GetDay(DayData.GetUnixTime() - 86400);
        float tempToday = today.temp_max;
        float pressureToday = today.pressure;
        float humidityToday = today.humidity;
        float tempChange = Math.Abs(tempToday - yest.temp_max);
        float pressureChange = Math.Abs(pressureToday - yest.pressure);
        float humidityChange = Math.Abs(humidityToday - yest.humidity);

        //*******************************************************//
        //Everything above here is accessed from sensors/database//
        //*******************************************************//

        float tempDiff = averageTemp - mAverageTemp;
        float pressureDiff = averagePressure - mAveragePressure;
        float humidityDiff = averageHumidity - mAverageHumidity;
        float tempChangeDiff = averageTempChange - mAverageTempChange;
        float pressureChangeDiff = averagePressureChange - mAveragePressureChange;
        float humidityChangeDiff = averageHumidityChange - mAverageHumidityChange;

        if(humidityChangeDiff < 0) { humidityChangeDiff *= -1; }
        if (tempChangeDiff < 0) { tempChangeDiff *= -1; }
        if (pressureChangeDiff < 0) { pressureChangeDiff *= -1; }
        if (tempDiff < 0) { tempDiff *= -1; }
        if(pressureDiff < 0) { pressureDiff *= -1; }
        if(humidityDiff < 0) { humidityDiff *= -1; }


        //Initiate the flags
        int flagsTriggered = 0;
        int totalFlags = 0;
        bool flagTemp = false;
        bool flagTempChange = false;
        bool flagHumidity = false;
        bool flagHumidityChange = false;
        bool flagPressure = false;
        bool flagPressureChange = false;

        //Find difference in todays data vs the average day
        bool tempOff = false;
        bool pressureOff = false;
        bool humidityOff = false;
        bool tempChangeOff = false;
        bool humidityChangeOff = false;
        bool pressureChangeOff = false;

        if (Math.Abs(tempToday - averageTemp) > 3)
            tempOff = true;
        if (Math.Abs(pressureToday - averagePressure) > 200)
            pressureOff = true;
        if (Math.Abs(humidityToday - averageHumidity) > 10)
            humidityOff = true;
        if (Math.Abs(tempChange - averageTempChange) > 3)
            tempChangeOff = true;
        if (Math.Abs(pressureChange - averagePressureChange) > 200)
            pressureChangeOff = true;
        if (Math.Abs(humidityChange - averageHumidityChange) > 10)
            humidityChangeOff = true;
        //TODO: Send updated info to the database

        //Set Flags
        if (tempDiff > 3) {
            flagTemp = true;
            totalFlags++;
        }
        if(pressureDiff > 200)
        {
            flagPressure = true;
            totalFlags++;
        }
        if(humidityDiff > 10)
        {
            flagHumidity = true;
            totalFlags++;
        }
        if(tempChangeDiff > 3)
        {
            flagTempChange = true;
            totalFlags++;
        }
        if(pressureChangeDiff > 200)
        {
            flagPressureChange = true;
            totalFlags++;
        }
        if(humidityChangeDiff > 10)
        {
            flagHumidityChange = true;
            totalFlags++;
        }

        //Examine Personal Risk
        if(days < 10 || migraineDays < 3 || totalFlags == 0)
        {
            PersonalRisk = -1;
        }
        else
        {
            if(flagTemp && tempOff)
                flagsTriggered++;
            if(flagHumidity && humidityOff)
                flagsTriggered++;
            if(flagPressure && pressureOff)
                flagsTriggered++;
            if(flagTempChange && tempChangeOff)
                flagsTriggered++;
            if (flagHumidityChange && humidityChangeOff)
                flagsTriggered++;
            if(flagPressureChange && pressureChangeOff)
                flagsTriggered++;


            //Personal Risk depends on amount of flags triggered compared to total flags
            PersonalRisk = (100 * flagsTriggered) / totalFlags;
        }

        //Examine General Risk Today
        generalRisk = 0;
        int totalFacts = 6;
        if (tempOff)
            generalRisk++;
        if (humidityOff)
            generalRisk++;
        if (pressureOff)
            generalRisk++;
        if (tempChangeOff)
            generalRisk++;
        if (humidityChangeOff)
            generalRisk++;
        if (pressureChangeOff)
            generalRisk++;

        generalRisk = (100 * generalRisk) / totalFacts;


    }

}
