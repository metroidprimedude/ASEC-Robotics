#ifndef __SPIUART_H__
#define __SPIUART_H__

#include "Configuration.h"
#include "_Spi.h"

#define ENABLE_BULK_TRANSFERS 0

#define THR          0x00 << 3
#define RHR          0x00 << 3
#define IER          0x01 << 3
#define FCR          0x02 << 3
#define IIR          0x02 << 3
#define LCR          0x03 << 3
#define MCR          0x04 << 3
#define LSR          0x05 << 3
#define MSR          0x06 << 3
#define SPR          0x07 << 3
#define TXLVL        0x08 << 3
#define RXLVL        0x09 << 3
#define DLAB         0x80 << 3
#define IODIR        0x0A << 3
#define IOSTATE      0x0B << 3
#define IOINTMSK     0x0C << 3
#define IOCTRL       0x0E << 3
#define EFCR         0x0F << 3

#define DLL          0x00 << 3
#define DLM          0x01 << 3
#define EFR          0x02 << 3
#define XON1         0x04 << 3    
#define XON2         0x05 << 3
#define XOFF1        0x06 << 3
#define XOFF2        0x07 << 3


#define SPI_READ_MODE_FLAG 0x80
#define BAUD_RATE_DEFAULT 9600


class SpiUartDevice : public SpiDevice, public Print 
{    
    public:
        void begin(unsigned long baudrate = BAUD_RATE_DEFAULT);
        byte available();
        int read();
        void write(byte value);
        void write(const char *str);
#if ENABLE_BULK_TRANSFERS
        void write(const uint8_t *buffer, size_t size);
#else
        using Print::write;
#endif
        void flush();

        void ioSetDirection(unsigned char bits);
        void ioSetState(unsigned char bits);
    
    private:
        void writeRegister(byte registerAddress, byte data);
        byte readRegister(byte registerAddress);
        void initUart(unsigned long baudrate);
        void configureUart(unsigned long baudrate);    
        void setBaudRate(unsigned long baudrate);
        boolean uartConnected();
};

#endif