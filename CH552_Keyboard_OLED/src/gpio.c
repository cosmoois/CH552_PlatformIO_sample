/********************************** (C) COPYRIGHT *******************************
* File Name          : GPIO.C
* Author             : WCH
* Version            : V1.0
* Date               : 2017/01/20
* Description        : CH554 IO インターフェースと割り込み機能設定  
*******************************************************************************/

#include <stdio.h>
#include "ch554_platform.h"
#include "gpio.h"

// #pragma  NOAREGS

/*******************************************************************************
* Function Name  : Port1Cfg()
* Description    : ポート1の構成
* Input          : Mode  0 = フローティング入力、プルアップなし
                         1 = プッシュプル入力および出力
                         2 = オープンドレイン入力および出力、プルアップなし
                         3 = クラス51モード、オープンドレイン入力および出力、プルアップ付き、内部回路はレベル上昇を低から高に加速できます	
                   ,UINT8 Pin	(0-7)											 
* Output         : None
* Return         : None
*******************************************************************************/
void Port1Cfg(uint8_t Mode, uint8_t Pin)
{
  switch(Mode){
    case 0:
      P1_MOD_OC = P1_MOD_OC & ~(1<<Pin);
      P1_DIR_PU = P1_DIR_PU &	~(1<<Pin);	
      break;
    case 1:
      P1_MOD_OC = P1_MOD_OC & ~(1<<Pin);
      P1_DIR_PU = P1_DIR_PU |	(1<<Pin);				
      break;		
    case 2:
      P1_MOD_OC = P1_MOD_OC | (1<<Pin);
      P1_DIR_PU = P1_DIR_PU &	~(1<<Pin);				
      break;		
    case 3:
      P1_MOD_OC = P1_MOD_OC | (1<<Pin);
      P1_DIR_PU = P1_DIR_PU |	(1<<Pin);			
      break;
    default:
      break;			
  }
}

/*******************************************************************************
* Function Name  : Port3Cfg()
* Description    : ポート3の構成
* Input          : Mode  0 = フローティング入力、プルアップなし
                         1 = プッシュプル入力および出力
                         2 = オープンドレイン入力および出力、プルアップなし
                         3 = クラス51モード、オープンドレイン入力および出力、プルアップ付き、内部回路はレベル上昇を低から高に加速できます	
                   ,UINT8 Pin	(0-7)											 
* Output         : None
* Return         : None
*******************************************************************************/
void Port3Cfg(uint8_t Mode, uint8_t Pin)
{
  switch(Mode){
    case 0:
      P3_MOD_OC = P3_MOD_OC & ~(1<<Pin);
      P3_DIR_PU = P3_DIR_PU &	~(1<<Pin);	
      break;
    case 1:
      P3_MOD_OC = P3_MOD_OC & ~(1<<Pin);
      P3_DIR_PU = P3_DIR_PU |	(1<<Pin);				
      break;		
    case 2:
      P3_MOD_OC = P3_MOD_OC | (1<<Pin);
      P3_DIR_PU = P3_DIR_PU &	~(1<<Pin);				
      break;		
    case 3:
      P3_MOD_OC = P3_MOD_OC | (1<<Pin);
      P3_DIR_PU = P3_DIR_PU |	(1<<Pin);			
      break;
    default:
      break;			
  }
}

/*******************************************************************************
* Function Name  : GPIOInterruptCfg()
* Description    : GPIO割り込み構成
* Input          : None									 
* Output         : None
* Return         : None
*******************************************************************************/
void GPIOInterruptCfg()
{
   GPIO_IE &= ~bIE_IO_EDGE;                                                    // 高/低レベルトリガー
//    GPIO_IE |= bIE_IO_EDGE;                                                  // 立ち上がり/立ち下がりトリガー
//    GPIO_IE |= bIE_RXD1_LO;                                                     // RXD1ローレベルまたは立ち下がりエッジトリガー
   GPIO_IE |= bIE_P1_5_LO | bIE_P1_4_LO | bIE_P1_3_LO | bIE_RST_HI;            
   //P15 \ P14\P13低レベルトリガー;RST高レベルトリガー
//    GPIO_IE |= bIE_P3_1_LO;                                                     // P31低レベルまたは立ち下がりエッジトリガー
//    GPIO_IE |= bIE_RXD0_LO;                                                     // RXD0ローレベルまたは立ち下がりエッジトリガー
   IE_GPIO  = 1;                                                               // GPIO割り込みが有効
}

#ifdef GPIO_INTERRUPT
/*******************************************************************************
* Function Name  : GPIOInterrupt(void)
* Description    : GPIO割り込みサービスルーチン
*******************************************************************************/
void	GPIOInterrupt( void ) interrupt INT_NO_GPIO  using 1                      // レジスタバンク1を使用したGPIO割り込みサービスルーチン
{ 
#ifdef DE_PRINTF
      printf("GPIO_STATUS: %02x\n",(UINT16)(PIN_FUNC&bIO_INT_ACT));
#endif
}
#endif
