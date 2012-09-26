#ifndef __WIFLY_DEVICE_H__
#define __WIFLY_DEVICE_H__

#include "Configuration.h"

#define DEFAULT_SERVER_PORT 80

class WiFlyDevice 
{
    public:
        WiFlyDevice(SpiUartDevice& theUart);
        void begin();

        boolean join(const char *ssid);
        boolean join(const char *ssid, const char *passphrase, 
		boolean isWPA = true);

        const char * ip();
        
    private:
        SpiUartDevice& uart;

        boolean serverConnectionActive;

        uint16_t serverPort;            
        
        void attemptSwitchToCommandMode();
        void switchToCommandMode();
        void reboot();
        void requireFlowControl();
        void setConfiguration();
        boolean sendCommand(const char *command, boolean isMultipartCommand, const char *expectedResponse);
        void waitForResponse(const char *toMatch);
        void skipRemainderOfResponse();
        boolean responseMatched(const char *toMatch);

        boolean findInResponse(const char *toMatch, unsigned int timeOut);
        boolean enterCommandMode(boolean isAfterBoot = false);
        boolean softwareReboot(boolean isAfterBoot);
        boolean hardwareReboot();

        friend class Client;
        friend class Server;
};

#endif