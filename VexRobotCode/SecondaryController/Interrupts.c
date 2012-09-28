#ifndef ASEC_INCLUDE
#define ASEC_INCLUDE

#include "Main.h"
#include "Interrupts.h"

#endif

#define LIGHT_SENSOR_IO in1 // The I/O port where the light sensor is connected
#define LEFT_LIFT_IO in2 // The I/O port where the left lift sensor is connected
#define RIGHT_LIFT_IO in3 // The I/O port where the left lift sensor is connected
#define REAR_LIFT_IO in4 // The I/O port where the left lift sensor is connected

int LiftInterrupt(void)
{
	if(SensorValue[LEFT_LIFT_IO] && SensorValue[RIGHT_LIFT_IO] && SensorValue[REAR_LIFT_IO]) {
		SensorValue[in8] = 1;
		SensorValue[in9] = 0;
		return 0;
		} else {
		SensorValue[in8] = 0;
		SensorValue[in9] = 1;
		return 1;
	}
}

int RxInterrupt(void)
{
	if(SensorValue(LIGHT_SENSOR_IO) > 600) {
		SensorValue[in6] = 0;
		SensorValue[in7] = 1;
		return 1;
	} else {
		SensorValue[in6] = 1;
		SensorValue[in7] = 0;
		return 0;
	}
}
