#ifndef ASEC_TYPES
#define ASEC_TYPES

#include "Types.h"

#endif

int CalculatePower(void);

void ExecuteMovementScheme(void);

void ExecuteObstructedMovementScheme(void);

void FrontBlockedMovement(void);

void RearBlockedMovement(void);

void InitializeMovement(void);

void SetMotors(char left, char right);

void StopMotors();
