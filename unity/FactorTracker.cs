using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class FactorTracker : MonoBehaviour
{

    int mAverageTemp;
    int mAveragePressure;
    int mAverageHumidity;
    int mAverageTempChange;
    int mAveragePressureChange;
    int mAverageHumidityChange;

    int migraineDays;

    int tempToday;
    int humidityToday;
    int pressureToday;
    int tempChange;
    int pressureChange;
    int humidityChange;

    bool migraineReported;
    bool migraineUnreported;


    // Start is called before the first frame update
    void Start()
    {
        //TODO: Read data from database(Days tracked, average)
        //TODO: Read data from sensors

        tempToday = 0;
        int tempYesterday = 0;
        humidityToday = 0;
        int humidityYesterday = 0;
        pressureToday = 0;
        int pressureYesterday = 0;
        pressureChange = pressureToday - pressureYesterday;
        if(pressureChange < 0) { pressureChange *= -1; }
        tempChange = tempToday - tempYesterday;
        if(tempChange < 0) { tempChange *= -1; }
        humidityChange = humidityToday - humidityYesterday;
        if(humidityChange < 0) { humidityChange *= -1; }

        int averageTemp = 0;
        int averagePressure = 0;
        int averageHumidity = 0;
        int averageTempChange = 0;
        int averagePressureChange = 0;
        int averageHumidityChange = 0;

        mAverageTemp = 0;
        mAveragePressure = 0;
        mAverageHumidity = 0;
        mAverageTempChange = 0;
        mAveragePressureChange = 0;
        mAverageHumidityChange = 0;

        int days = 0;
        int migraineDays = 0;
        //*******************************************************//
        //Everything above here is accessed from sensors/database//
        //*******************************************************//

        int tempDiff = averageTemp - mAverageTemp;
        int pressureDiff = averagePressure - mAveragePressure;
        int humidityDiff = averageHumidity - mAverageHumidity;
        int tempChangeDiff = averageTempChange - mAverageTempChange;
        int pressureChangeDiff = averagePressureChange - mAveragePressureChange;
        int humidityChangeDiff = averageHumidityChange - mAverageHumidityChange;

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

        //Update Averages and days
        averageTemp = ((averageTemp * days) + tempToday) / (days + 1);
        averagePressure = ((averagePressure * days) + pressureToday) / (days + 1);
        averageHumidity = ((averageHumidity * days) + humidityToday) / (days + 1);
        averageTempChange = ((averageTempChange * days) + tempChange) / (days + 1);
        averagePressureChange = ((averagePressureChange * days) + pressureChange) / (days + 1);
        averageHumidityChange = ((averageHumidityChange * days) + humidityChange) / (days + 1);
        days++;

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
        if(days < 14 || migraineDays < 5 || totalFlags == 0)
        {
            //Notfiy user that personal risk assessment is unavailable
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

            //Notify User of their personal risk of a migraine today
            //Personal Risk depends on amount of flags triggered compared to total flags
            int PersonalRisk = flagsTriggered / totalFlags;
        }

        //Examine General Risk Today
        int generalRisk = 0;
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

        //Notfiy User of the general risk today

    }

    // Update is called once per frame
    void Update()
    {
        //When User reports or unreports migraine
        //TODO: Signal when migraine is reported
        if (migraineReported)
        {
            mAverageTemp = ((mAverageTemp * migraineDays) + tempToday) / (migraineDays + 1);
            mAverageHumidity = ((mAverageHumidity * migraineDays) + humidityToday) / (migraineDays + 1);
            mAveragePressure = ((mAveragePressure * migraineDays) + pressureToday) / (migraineDays + 1);
            mAverageTempChange = ((mAverageTempChange * migraineDays) + tempChange) / (migraineDays + 1);
            mAveragePressureChange = ((mAveragePressureChange * migraineDays) + pressureChange) / (migraineDays + 1);
            mAverageHumidityChange = ((mAverageHumidityChange * migraineDays) + humidityChange) / (migraineDays + 1);

            migraineDays++;

            //TODO: Update database with new mAverages
        }

        //TODO: Signal when migraine is unreported
        if (migraineUnreported)
        {
            mAverageTemp = ((mAverageTemp * migraineDays) - tempToday) / (migraineDays - 1);
            mAverageHumidity = ((mAverageHumidity * migraineDays) - humidityToday) / (migraineDays - 1);
            mAveragePressure = ((mAveragePressure * migraineDays) - pressureToday) / (migraineDays - 1);
            mAverageTempChange = ((mAverageTempChange * migraineDays) - tempChange) / (migraineDays - 1);
            mAveragePressureChange = ((mAveragePressureChange * migraineDays) - pressureChange) / (migraineDays - 1);
            mAverageHumidityChange = ((mAverageHumidityChange * migraineDays) - humidityChange) / (migraineDays - 1);

            migraineDays--;

            //TODO: Update database with new mAverages
        }
    }
}
