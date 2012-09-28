#ifndef ASEC_INCLUDE
#define ASEC_INCLUDE

#include "Main.h"
#include "Interrupts.h"
#include "Movement.h"
#include "Stability.h"

#endif

#define RX_IO  in15 // The I/O pin that communicates an RX Interrupt from the safety controller
#define FRONT_ULTRASONIC_IO in1 // The I/O pin that the forward facing ultrasonic sensor is connected to
#define REAR_ULTRASONIC_IO in2 // The I/O pin that the forward facing ultrasonic sensor is connected to
#define US_HIST_LEN 15

void InitializeInterrupts(void)
{
}

int SControllerInterrupt(void)
{
	if(!SensorValue[RX_IO])	{
		// Stop the robot and wait for 100ms, then return an Interrupt code.
		StopMotors();
		wait1Msec(100);
		return 1;
	} else {
		return 0;
	}
}

int UltrasonicInterrupt(void)
{
	// The ultrasonic sensor reports values in inches
	#define FRONT_OFFSET 3
	#define FRONT_NEAR 30 + FRONT_OFFSET
	#define FRONT_CLOSE 9 + FRONT_OFFSET
	#define REAR_OFFSET 3
	#define REAR_NEAR 30 + REAR_OFFSET
	#define REAR_CLOSE 9 + REAR_OFFSET

	static int frontUSHistory[US_HIST_LEN];
	static int rearUSHistory[US_HIST_LEN];
	static char ushIndex;
	
	static char plsCont;
	static int plsCount;

	char i;
	char interVal = 0;
	int fMinVal = FRONT_NEAR + 1;
	int rMinVal = REAR_NEAR + 1;
	char override = vexRT[Ch6];

	frontUSHistory[ushIndex] = SensorValue[FRONT_ULTRASONIC_IO];
	rearUSHistory[ushIndex] = SensorValue[REAR_ULTRASONIC_IO];
	ushIndex = ushIndex + 1 >= US_HIST_LEN ? 0 : ushIndex + 1;
	for(i = 0; i < US_HIST_LEN; i++) {
		fMinVal = frontUSHistory[i] > 0 && frontUSHistory[i] < fMinVal ? frontUSHistory[i] : fMinVal;
		rMinVal = rearUSHistory[i] > 0 && rearUSHistory[i] < rMinVal ? rearUSHistory[i] : rMinVal;
	}

	// Take action based on ultrasonic sensors
	if(override) {
		// If either of the Channel 6 buttons is pressed, allow full
		// manual control.
		SensorValue[in14] = 0;
		return 0;
	} else if(fMinVal < FRONT_CLOSE) {
		FrontBlockedMovement();
		SensorValue[in3] = 0;
		SensorValue[in4] = 1;
		SensorValue[in14] = 1;
		interVal = 1;
	} else if(rMinVal < REAR_CLOSE) {
		RearBlockedMovement();
		SensorValue[in3] = 0;
		SensorValue[in4] = 1;
		SensorValue[in14] = 1;
		interVal = 1;
	} else if(fMinVal < FRONT_NEAR || rMinVal < REAR_NEAR) {
		ExecuteObstructedMovementScheme();
		plsCount++;
		if(plsCount > 10)
			plsCount = 0;
		if(!plsCount)
			plsCont = !plsCont;
		SensorValue[in4] = plsCont;
		SensorValue[in3] = 0;
		SensorValue[in14] = 0;
		interVal = 1;
	} else {
		SensorValue[in14] = 0;
		SensorValue[in3] = 1;
		SensorValue[in4] = 0;
	}
	return interVal;
}

int StabilityInterrupt(void)
{
	return 0;
}
