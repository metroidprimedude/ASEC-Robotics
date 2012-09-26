#include "SpiUart.h"

#define EFR_ENABLE_CTS 1 << 7
#define EFR_ENABLE_RTS 1 << 6
#define EFR_ENABLE_ENHANCED_FUNCTIONS 1 << 4


#define LCR_ENABLE_DIVISOR_LATCH 1 << 7


#ifndef USE_14_MHZ_CRYSTAL
#define USE_14_MHZ_CRYSTAL true // true (14MHz) , false (12 MHz)
#endif

#if USE_14_MHZ_CRYSTAL
#define XTAL_FREQUENCY 14745600UL // On-board crystal (New mid-2010 Version)
#else
#define XTAL_FREQUENCY 12288000UL // On-board crystal (Original Version)
#endif


#define BAUD_RATE_DIVISOR(baud) ((XTAL_FREQUENCY/PRESCALER)/(baud*16UL))


struct SPI_UART_cfg 
{
    char DataFormat;
    char Flow;
};

struct SPI_UART_cfg SPI_Uart_config = {
    0x03,
    EFR_ENABLE_CTS | EFR_ENABLE_RTS | EFR_ENABLE_ENHANCED_FUNCTIONS
};

void SpiUartDevice::begin(unsigned long baudrate) 
{
    SpiDevice::begin();
    initUart(baudrate);
}

void SpiUartDevice::initUart(unsigned long baudrate) 
{
    configureUart(baudrate);
    
    if(!uartConnected())
	{ 
        while(1) 
		{
            // Lock up if we fail to initialise SPI UART bridge.
        }; 
    }
}

void SpiUartDevice::setBaudRate(unsigned long baudrate) 
{
    unsigned long divisor = BAUD_RATE_DIVISOR(baudrate);

    writeRegister(LCR, LCR_ENABLE_DIVISOR_LATCH); // "Program baudrate"
    writeRegister(DLL, lowByte(divisor));
    writeRegister(DLM, highByte(divisor)); 
}

void SpiUartDevice::configureUart(unsigned long baudrate) 
{
    setBaudRate(baudrate);

    writeRegister(LCR, 0xBF); // access EFR register
    writeRegister(EFR, SPI_Uart_config.Flow); // enable enhanced registers
    writeRegister(LCR, SPI_Uart_config.DataFormat); // 8 data bit, 1 stop bit, no parity
    writeRegister(FCR, 0x06); // reset TXFIFO, reset RXFIFO, non FIFO mode
    writeRegister(FCR, 0x01); // enable FIFO mode     
}

boolean SpiUartDevice::uartConnected()
{
    const char TEST_CHARACTER = 'H';    
    writeRegister(SPR, TEST_CHARACTER);
    return (readRegister(SPR) == TEST_CHARACTER);
}

void SpiUartDevice::writeRegister(byte registerAddress, byte data)
{
    select();
    transfer(registerAddress);
    transfer(data);
    deselect();
}

byte SpiUartDevice::readRegister(byte registerAddress)
{
    const byte SPI_DUMMY_BYTE = 0xFF; 
    
    char result;

    select();
    transfer(SPI_READ_MODE_FLAG | registerAddress);
    result = transfer(SPI_DUMMY_BYTE);
    deselect();
    return result;    
}


byte SpiUartDevice::available() { return readRegister(RXLVL); }

int SpiUartDevice::read() 
{
    if (!available())
        return -1;
    
    return readRegister(RHR);
}

void SpiUartDevice::write(byte value) 
{
    while (readRegister(TXLVL) == 0) 
	{
        // Wait for space in TX buffer
    };
    writeRegister(THR, value); 
}

void SpiUartDevice::write(const char *str) 
{
    write((const uint8_t *) str, strlen(str));    
    while (readRegister(TXLVL) < 64) 
	{
        // Wait for empty TX buffer (slow)
    };
}

#if ENABLE_BULK_TRANSFERS
void SpiUartDevice::write(const uint8_t *buffer, size_t size)
{
    select();
    transfer(THR);

    while(size > 16) 
	{
        transfer_bulk(buffer, 16);
        size -= 16;
        buffer += 16;
    }
    transfer_bulk(buffer, size);

    deselect();
}
#endif

void SpiUartDevice::flush() 
{
    while(available() > 0)
        read();
}


void SpiUartDevice::ioSetDirection(unsigned char bits) { writeRegister(IODIR, bits); }


void SpiUartDevice::ioSetState(unsigned char bits) { writeRegister(IOSTATE, bits); }