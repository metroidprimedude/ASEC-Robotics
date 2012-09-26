#include "WiFly.h"
#include "Debug.h"

#define DEBUG_LEVEL 0

boolean WiFlyDevice::findInResponse(const char *toMatch, unsigned int timeOut = 0)
{
    int byteRead;    
    unsigned int timeOutTarget; // in milliseconds    
    
    DEBUG_LOG(1, "Entered findInResponse");
    DEBUG_LOG(2, "Want to match:");
    DEBUG_LOG(2, toMatch);
    DEBUG_LOG(3, "Found:");

    for (unsigned int offset = 0; offset < strlen(toMatch); offset++)
	{
        timeOutTarget = millis() + timeOut; // Doesn't handle timer wrapping

        while (!uart.available()) 
		{
            // Wait, with optional time out.
            if (timeOut > 0) 
                if (millis() > timeOutTarget)
                    return false;

            delay(1); // This seems to improve reliability slightly
        }

        byteRead = uart.read();

        delay(1);

        DEBUG_LOG(5, "Offset:");
        DEBUG_LOG(5, offset);
        DEBUG_LOG(3, (char) byteRead);
        DEBUG_LOG(4, byteRead);

        if (byteRead != toMatch[offset]) 
		{
            offset = 0;
            if (byteRead != toMatch[offset])
                offset = -1;
            continue;
        }
    }    
    return true;
}

boolean WiFlyDevice::responseMatched(const char *toMatch) 
{
    boolean matchFound = true;
    
    DEBUG_LOG(3, "Entered responseMatched");

    for (unsigned int offset = 0; offset < strlen(toMatch); offset++)
	{
        while (!uart.available()) 
		{
            // Wait -- no timeout
        }
        if (uart.read() != toMatch[offset])
		{
            matchFound = false;
            break;
        }
    }
    return matchFound;
}

#define COMMAND_MODE_ENTER_RETRY_ATTEMPTS 5
#define COMMAND_MODE_GUARD_TIME 250 // in milliseconds

boolean WiFlyDevice::enterCommandMode(boolean isAfterBoot) 
{
    DEBUG_LOG(1, "Entered enterCommandMode");

    for (int retryCount = 0; retryCount < COMMAND_MODE_ENTER_RETRY_ATTEMPTS; retryCount++) 
	{
        if (isAfterBoot)
		{
            delay(1000); // This delay is so characters aren't missed after a reboot.
        }
    
        delay(COMMAND_MODE_GUARD_TIME);        
        uart.print("$$$");        
        delay(COMMAND_MODE_GUARD_TIME);

        uart.println();
        uart.println();    

        uart.println("ver");

        if (findInResponse("\r\nWiFly Ver", 1000))
            return true;
    }
    return false;
}

void WiFlyDevice::skipRemainderOfResponse() 
{
    DEBUG_LOG(3, "Entered skipRemainderOfResponse");

	while (!(uart.available() && (uart.read() == '\n'))) 
	{
		// Skip remainder of response
	}
}    


void WiFlyDevice::waitForResponse(const char *toMatch)
{
     while(!responseMatched(toMatch))
         skipRemainderOfResponse();
}

WiFlyDevice::WiFlyDevice(SpiUartDevice& theUart) : uart (theUart) 
{
    serverPort = DEFAULT_SERVER_PORT;
    serverConnectionActive = false;
}

void WiFlyDevice::begin() 
{
    DEBUG_LOG(1, "Entered WiFlyDevice::begin()");

    uart.begin();
    reboot(); // Reboot to get device into known state
    requireFlowControl();
    setConfiguration();
}

#define SOFTWARE_REBOOT_RETRY_ATTEMPTS 5

boolean WiFlyDevice::softwareReboot(boolean isAfterBoot = true) 
{
    DEBUG_LOG(1, "Entered softwareReboot");

    for (int retryCount = 0; retryCount < SOFTWARE_REBOOT_RETRY_ATTEMPTS; retryCount++)
	{ 
        if (!enterCommandMode(isAfterBoot))
            return false;
    
        uart.println("reboot");

        if (findInResponse("*READY*", 2000))
            return true;
    }
    
    return false;
}

boolean WiFlyDevice::hardwareReboot()
{
    uart.ioSetDirection(0b00000010);
    uart.ioSetState(0b00000000);
    delay(1);
    uart.ioSetState(0b00000010);

    return findInResponse("*READY*", 2000);
}

#if USE_HARDWARE_RESET
#define REBOOT hardwareReboot
#else
#define REBOOT softwareReboot
#endif

void WiFlyDevice::reboot() 
{
    DEBUG_LOG(1, "Entered reboot");

    if (!REBOOT())
	{
        DEBUG_LOG(1, "Failed to reboot. Halting.");
        while (1) {}; // Hang. TODO: Handle differently?
    }
}

boolean WiFlyDevice::sendCommand(const char *command, boolean isMultipartCommand = false, const char *expectedResponse = "AOK") 
{
    uart.print(command);
    
    if (!isMultipartCommand) 
	{
        uart.flush();
        uart.println();
    
        waitForResponse(expectedResponse);
    }        
    
    return true;
}

void WiFlyDevice::requireFlowControl() 
{
    DEBUG_LOG(1, "Entered requireFlowControl");
    enterCommandMode();
    sendCommand("get uart", false, "Flow=0x");

    while (!uart.available()) 
	{
        // Wait to ensure we have the full response
    }

    char flowControlState = uart.read();

    uart.flush();

    if (flowControlState == '1')
        return;

    sendCommand("set uart flow 1");
    sendCommand("save", false, "Storing in config");
    sendCommand("get uart", false, "Flow=0x1");

    reboot();
}

void WiFlyDevice::setConfiguration() 
{
    enterCommandMode();

    sendCommand("set wlan join 0");
    sendCommand("set ip localport ", true);
    uart.print(serverPort);
    sendCommand("");

    sendCommand("set comm remote 0");
}

boolean WiFlyDevice::join(const char *ssid) 
{
    sendCommand("join ", true);    
    if (sendCommand(ssid, false, "Associated!")) 
	{
        waitForResponse("Listen on ");
        skipRemainderOfResponse();
        return true;
    }
    return false;
}

boolean WiFlyDevice::join(const char *ssid, const char *passphrase, boolean isWPA)
{
    sendCommand("set wlan ", true);

    if (isWPA)
        sendCommand("passphrase ", true);
    else
        sendCommand("key ", true);
    
    sendCommand(passphrase);        
    
    return join(ssid);
}

#define IP_ADDRESS_BUFFER_SIZE 16 // "255.255.255.255\0"

const char * WiFlyDevice::ip() 
{
    static char ip[IP_ADDRESS_BUFFER_SIZE] = "";
    
    enterCommandMode();

    sendCommand("get ip", false, "IP=");

    char newChar;
    byte offset = 0;

    while (offset < IP_ADDRESS_BUFFER_SIZE)
	{
        newChar = uart.read();

        if (newChar == ':') 
		{
            ip[offset] = '\x00';
            break;
        } else if (newChar != -1) 
		{
            ip[offset] = newChar;
            offset++;
        }
    }

    ip[IP_ADDRESS_BUFFER_SIZE-1] = '\x00';

    waitForResponse("<");
    while (uart.read() != ' ')
	{
        // Skip the prompt
    }

    uart.println("exit");
    return ip;
}

SpiUartDevice SpiSerial;
WiFlyDevice WiFly(SpiSerial);