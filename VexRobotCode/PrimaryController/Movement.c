#ifndef ASEC_INCLUDE
#define ASEC_INCLUDE

#include "Main.h"
#include "Interrupts.h"
#include "Movement.h"
#include "Stability.h"

#endif

#define POWER_CHANNEL Ch2
#define STEER_CHANNEL Ch1

#define DEADZONE_RANGE 8
#define LOW_DEADZONE 0 - DEADZONE_RANGE
#define HIGH_DEADZONE 0 + DEADZONE_RANGE

#define LEFT_MOTOR  port8
#define RIGHT_MOTOR  port7
#define FLIP_LEFT 0
#define FLIP_RIGHT 1

int CalculatePower(void)
{
	char power;
	char steer;
	char leftMotor;
	char rightMotor;

	power = vexRT[POWER_CHANNEL];
	steer = vexRT[STEER_CHANNEL];

	// Calculate left and right motor values
	// motor values depend on the combined values of power and steer
	if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE &&
	   steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE)	{
		// both axes centered, stop the robot
		leftMotor = 0;
		rightMotor = 0;
	} else if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE) {
		// the power control is centered, so just pivot in place
		leftMotor = -steer;
		rightMotor = steer;
	} else if(steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE) {
		// the steering control is centered, so give both motors equal power
		leftMotor = power;
		rightMotor = power;
	} else {
		// If we've gotten this far, we want to both move and steer
		if(steer > HIGH_DEADZONE) {
			// We want to go right
			leftMotor = power;
			rightMotor = power * (127 - steer) / 127;
		} else {
			// We want to go left
			leftMotor = power * (127 + steer) / 127;
			rightMotor = power;
		}
	}
	return 1;
}

void ExecuteMovementScheme(void)
{
	char power;
	char steer;
	char leftMotor;
	char rightMotor;

	power = vexRT[POWER_CHANNEL];
	steer = vexRT[STEER_CHANNEL];

	// Calculate left and right motor values
	// motor values depend on the combined values of power and steer
	if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE &&
	   steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE)	{
		// both axes centered, stop the robot
		leftMotor = 0;
		rightMotor = 0;
	} else if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE) {
		// the power control is centered, so just pivot in place
		leftMotor = -steer;
		rightMotor = steer;
	} else if(steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE) {
		// the steering control is centered, so give both motors equal power
		leftMotor = power;
		rightMotor = power;
	} else {
		// If we've gotten this far, we want to both move and steer
		if(steer > HIGH_DEADZONE) {
			// We want to go right
			leftMotor = power;
			rightMotor = power * (127 - steer) / 127;
		} else {
			// We want to go left
			leftMotor = power * (127 + steer) / 127;
			rightMotor = power;
		}
	}
	
	SetMotors(leftMotor, rightMotor);
}

void ExecuteObstructedMovementScheme(void)
{
	char power;
	char steer;
	char leftMotor;
	char rightMotor;

	power = vexRT[POWER_CHANNEL];
	steer = vexRT[STEER_CHANNEL];

	// Calculate left and right motor values
	// motor values depend on the combined values of power and steer
	if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE &&
	   steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE)	{
		// both axes centered, stop the robot
		leftMotor = 0;
		rightMotor = 0;
	} else if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE) {
		// the power control is centered, so just pivot in place
		leftMotor = -steer;
		rightMotor = steer;
	} else if(steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE) {
		// the steering control is centered, so give both motors equal power
		leftMotor = power;
		rightMotor = power;
	} else {
		// If we've gotten this far, we want to both move and steer
		if(steer > HIGH_DEADZONE) {
			// We want to go right
			leftMotor = power;
			rightMotor = power * (127 - steer) / 127;
		} else {
			// We want to go left
			leftMotor = power * (127 + steer) / 127;
			rightMotor = power;
		}
	}

	SetMotors(leftMotor / 2, rightMotor / 2);
}

void FrontBlockedMovement(void)
{
	char power;
	char steer;
	char leftMotor;
	char rightMotor;

	power = vexRT[POWER_CHANNEL];
	steer = vexRT[STEER_CHANNEL];

	// Calculate left and right motor values
	// motor values depend on the combined values of power and steer
	if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE &&
	   steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE)	{
		// both axes centered, stop the robot
		leftMotor = 0;
		rightMotor = 0;
	} else if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE) {
		// the power control is centered, so just pivot in place
		leftMotor = -steer;
		rightMotor = steer;
	} else if(steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE) {
		// the steering control is centered, so give both motors equal power
		leftMotor = power;
		rightMotor = power;
	} else {
		// If we've gotten this far, we want to both move and steer
		if(steer > HIGH_DEADZONE) {
			// We want to go right
			leftMotor = power;
			rightMotor = power * (127 - steer) / 127;
		} else {
			// We want to go left
			leftMotor = power * (127 + steer) / 127;
			rightMotor = power;
		}
	}
	
	if(leftMotor > 0)
		leftMotor = 0;
	if(rightMotor > 0)
		rightMotor = 0;

	SetMotors(leftMotor / 2, rightMotor / 2);
}

void RearBlockedMovement(void)
{
	char power;
	char steer;
	char leftMotor;
	char rightMotor;

	power = vexRT[POWER_CHANNEL];
	steer = vexRT[STEER_CHANNEL];

	// Calculate left and right motor values
	// motor values depend on the combined values of power and steer
	if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE &&
	   steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE)	{
		// both axes centered, stop the robot
		leftMotor = 0;
		rightMotor = 0;
	} else if(power >= LOW_DEADZONE && power <= HIGH_DEADZONE) {
		// the power control is centered, so just pivot in place
		leftMotor = -steer;
		rightMotor = steer;
	} else if(steer >= LOW_DEADZONE && steer <= HIGH_DEADZONE) {
		// the steering control is centered, so give both motors equal power
		leftMotor = power;
		rightMotor = power;
	} else {
		// If we've gotten this far, we want to both move and steer
		if(steer > HIGH_DEADZONE) {
			// We want to go right
			leftMotor = power;
			rightMotor = power * (127 - steer) / 127;
		} else {
			// We want to go left
			leftMotor = power * (127 + steer) / 127;
			rightMotor = power;
		}
	}
	
	if(leftMotor < 0)
		leftMotor = 0;
	if(rightMotor < 0)
		rightMotor = 0;

	SetMotors(leftMotor / 2, rightMotor / 2);
}

void InitializeMovement(void)
{
	return;
}

void SetMotors(char left, char right)
{
	left = FLIP_LEFT ? -left : left;
	right = FLIP_RIGHT ? -right : right;

	motor[LEFT_MOTOR] = left;
	motor[RIGHT_MOTOR] = right;
}

void StopMotors(void)
{
	motor[LEFT_MOTOR] = 0;
	motor[RIGHT_MOTOR] = 0;
}
