#include "ch554_platform.h"
#include "gpio.h"
#include "i2c.h"

xdata uint8_t I2C_Buf;

#define SDA_PIN 4
SBIT(sda_pin, 0xB0, SDA_PIN);
#define SCL_PIN 3
SBIT(scl_pin, 0xB0, SCL_PIN);

#define I2C_SDA()	(sda_pin==1)
#define I2C_SDA_H()	{sda_pin = 1;}
#define I2C_SDA_L()	{sda_pin = 0;}
#define I2C_SCL_H()	{scl_pin = 1;}
#define I2C_SCL_L()	{scl_pin = 0;}

void I2C_DELAY(void) {
	uint8_t i;
	for (i=0; i<3; i++);	// Frequency control
}

void I2C_Init(void) {
	Port3Cfg(2, SDA_PIN);
	Port3Cfg(2, SCL_PIN);

	I2C_SDA_H();
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
}

// Generate a I2C start sequence
void I2C_Send_Start(void) {
	// I2C_SCL = H, I2C_SDA = H
	I2C_SDA_H();
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SDA_L();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();	
	// I2C_SCL = L, I2C_SDA = L
}

// Write a byte (stored in I2C_Buf) to I2C bus and return the ACK status in I2C_Buf
// xxxx xxx(ACK bit)
// ACK bit = 0 -> ACK received
// ACK bit = 1 -> NACK received
void I2C_WriteByte(void) {
	// I2C_SCL = L
	if (I2C_Buf & 0x80) {I2C_SDA_H();} else {I2C_SDA_L();}
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();
	
	if (I2C_Buf & 0x40) {I2C_SDA_H();} else {I2C_SDA_L();}
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();
	
	if (I2C_Buf & 0x20) {I2C_SDA_H();} else {I2C_SDA_L();}
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();
	
	if (I2C_Buf & 0x10) {I2C_SDA_H();} else {I2C_SDA_L();}
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();
	
	if (I2C_Buf & 0x08) {I2C_SDA_H();} else {I2C_SDA_L();}
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();

	if (I2C_Buf & 0x04) {I2C_SDA_H();} else {I2C_SDA_L();}
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();

	if (I2C_Buf & 0x02) {I2C_SDA_H();} else {I2C_SDA_L();}
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();

	if (I2C_Buf & 0x01) {I2C_SDA_H();} else {I2C_SDA_L();}
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();
	
	I2C_SDA_H();
	I2C_DELAY();
	
	// Check ACK bit
	// I2C_SCL = L
	I2C_SCL_H();
	I2C_DELAY();
	
	I2C_Buf = 0;
	if (I2C_SDA())
		I2C_Buf = 1;
	
	I2C_SCL_L();
	I2C_DELAY();
}

// Read a byte from I2C bus and store the result into I2C_Buf
void I2C_ReadByte(void) {
	I2C_Buf = 0;
	
	// I2C_SCL = L
	I2C_SDA_H(); //Master release the bus
	I2C_DELAY();	

	I2C_SCL_H();
	I2C_DELAY();
	if (I2C_SDA())
		I2C_Buf |= 0x80;
	I2C_SCL_L();
	I2C_DELAY();
	

	I2C_SCL_H();
	I2C_DELAY();
	if (I2C_SDA())
		I2C_Buf |= 0x40;
	I2C_SCL_L();
	I2C_DELAY();

	I2C_SCL_H();
	I2C_DELAY();
	if (I2C_SDA())
		I2C_Buf |= 0x20;
	I2C_SCL_L();
	I2C_DELAY();

	I2C_SCL_H();
	I2C_DELAY();
	if (I2C_SDA())
		I2C_Buf |= 0x10;
	I2C_SCL_L();
	I2C_DELAY();

	I2C_SCL_H();
	I2C_DELAY();
	if (I2C_SDA())
		I2C_Buf |= 0x08;
	I2C_SCL_L();
	I2C_DELAY();

	I2C_SCL_H();
	I2C_DELAY();
	if (I2C_SDA())
		I2C_Buf |= 0x04;
	I2C_SCL_L();
	I2C_DELAY();

	I2C_SCL_H();
	I2C_DELAY();
	if (I2C_SDA())
		I2C_Buf |= 0x02;
	I2C_SCL_L();
	I2C_DELAY();

	I2C_SCL_H();
	I2C_DELAY();
	if (I2C_SDA())
		I2C_Buf |= 0x01;
	I2C_SCL_L();
	I2C_DELAY();
}

// Send a ACK to I2C bus
void I2C_Send_ACK(void) {
	// I2C_SCL = L	
	I2C_SDA_L();
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();	
}

// Send a NACK to I2C bus
void I2C_Send_NACK(void) {
	// I2C_SCL = L
	I2C_SDA_H();
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SCL_L();
	I2C_DELAY();	
}

// Generate a I2C stop condition, release the bus
void I2C_Send_Stop(void) {
	// I2C_SCL = L, I2C_SDA = H
	I2C_SDA_L();
	I2C_DELAY();
	I2C_SCL_H();
	I2C_DELAY();
	I2C_SDA_H(); //Master release the bus
	I2C_DELAY();
}
