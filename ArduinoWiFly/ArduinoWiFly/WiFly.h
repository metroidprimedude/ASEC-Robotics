#ifndef __WIFLY_H__
#define __WIFLY_H__

#include "SpiUart.h"
#include "WiFlyDevice.h"
#include "Client.h"
#include "Server.h"

#define WEP_MODE false
#define WPA_MODE true


extern SpiUartDevice SpiSerial;

extern WiFlyDevice WiFly;

#endif