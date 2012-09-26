#ifndef __WIFLY_CLIENT_H__
#define __WIFLY_CLIENT_H__

#include "Print.h"
#include "ParsedStream.h"
#include "WiFlyDevice.h"

class Client : public Print 
{
 public:
    Client(uint8_t *ip, uint16_t port);
    Client(const char* domain, uint16_t port);

    boolean connect();

    void write(byte value);
    void write(const char *str);
    void write(const uint8_t *buffer, size_t size);

    int available();
    int read();
    void flush(void);

    bool connected();
    void stop();

    operator bool();

 private:
    WiFlyDevice& _WiFly;

    uint8_t *_ip;
    uint16_t _port;

    const char *_domain;

    bool isOpen;

    ParsedStream stream;

    friend class Server;
};

#endif
