#include "WiFly.h"
#include "Client.h"

Client::Client(uint8_t *ip, uint16_t port) : _WiFly (WiFly), stream (ParsedStream(SpiSerial))
{
    _ip = ip;
    _port = port;
    _domain = NULL;

    isOpen = false;
}

Client::Client(const char* domain, uint16_t port) : _WiFly (WiFly), stream (ParsedStream(SpiSerial)) 
{
    _ip = NULL;
    _port = port;
    _domain = domain;

    isOpen = false;
}

void Client::write(byte value) { _WiFly.uart.write(value); }

void Client::write(const char *str) { _WiFly.uart.write(str); }

void Client::write(const uint8_t *buffer, size_t size) { _WiFly.uart.write(buffer, size); }

boolean Client::connect() 
{
    if (!this)
        return false;

    stream.reset();

    if ((_ip == NULL) && (_domain == NULL)) 
	{
        // This is a connection started by the Server class
        // so the connection is already established.
    } 
	else 
	{
        _WiFly.enterCommandMode();        
        _WiFly.sendCommand("open ", true, "" /* TODO: Remove this dummy value */);
        
        if (_ip != NULL) 
		{
            for (int index = 0; /* break inside loop*/ ; index++) 
			{
				_WiFly.uart.print(_ip[index], DEC);
				if (index == 3) {
					break;
				}
				_WiFly.uart.print('.');
			}
        }
		else if (_domain != NULL) 
		{
            _WiFly.uart.print(_domain);
        }
		else 
		{
            while (1) 
			{
				// This should never happen
            }
        }
        
        _WiFly.uart.print(" ");        
        _WiFly.uart.print(_port, DEC);        
        _WiFly.sendCommand("", false, "*OPEN*");
    }
    
    isOpen = true;

    return true;
}


int Client::available() 
{
    if (!isOpen)
        return 0;

    return stream.available();
}


int Client::read() 
{
    if (!isOpen)
        return -1;

    return stream.read();
}


void Client::flush(void)
{
    if (!isOpen)
        return;

    while (stream.available() > 0)
        stream.read();
}


bool Client::connected() 
{
    return isOpen && !stream.closed();
}


void Client::stop() 
{
    _WiFly.enterCommandMode();
    _WiFly.uart.println("close");

    _WiFly.uart.println("exit");
    _WiFly.waitForResponse("EXIT");
    _WiFly.skipRemainderOfResponse();

    stream.reset();

    isOpen = false;

    _WiFly.serverConnectionActive = false;
}


Client::operator bool() 
{
    return !((_ip == NULL) && (_domain == NULL) && (_port == 0));
}