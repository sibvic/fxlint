
#property indicator_chart_window

enum e_cycles{ Min_5=1, Min_15=2, Min_30=3, Min_60=4, Min_240=5, Daily=6, Weekly=7, Monthly=8 };

input  e_cycles Cycles                    = Daily;
extern int      Number_Of_Cycles          = 20;
extern bool     Show_Labels               = true;
extern bool     Draw_Cycles_Separator     = true;
extern color    High_and_Low_Color        = clrTeal;
extern color    Level_50_Color            = clrRed;
extern color    Level_25_and_75_Color     = clrLime;
extern color    Level_37_5_and_62_5_Color = clrDodgerBlue;
extern color    Cycles_Separator_Color    = clrYellow;

int Periodo, Minutes;

string IndicatorName;
string IndicatorObjPrefix;

string GenerateIndicatorName(const string target)
{
   string name = target;
   int try = 2;
   while (WindowFind(name) != -1)
   {
      name = target + " #" + IntegerToString(try++);
   }
   return name;
}

int init(){
   IndicatorName = GenerateIndicatorName("Traders Trinity");
   IndicatorObjPrefix = "__" + IndicatorName + "__";
   IndicatorShortName(IndicatorName);
   
   if (Check()) Alert("The Cycles selected for this Time Frame cannot be calculated");
   
   return(0);
}

int deinit(){
   ObjectsDeleteAll(ChartID(), IndicatorObjPrefix);
   return(0);
}

int start()
  {
   
   int i;
   double Range, Level_0, Level_25, Level_375, Level_50, Level_625, Level_75, Level_100;
   datetime Line_Start, Line_End;
   bool Draw_Label;
   
   if (Check()==false){
   
   for(i=Number_Of_Cycles; i>=0; i--){
      
      if (Cycles==1){ Periodo = PERIOD_M5;  Minutes = 5;     }
      if (Cycles==2){ Periodo = PERIOD_M15; Minutes = 15;    }
      if (Cycles==3){ Periodo = PERIOD_M30; Minutes = 30;    }
      if (Cycles==4){ Periodo = PERIOD_H1;  Minutes = 60;    }
      if (Cycles==5){ Periodo = PERIOD_H4;  Minutes = 240;   }
      if (Cycles==6){ Periodo = PERIOD_D1;  Minutes = 1440;  }
      if (Cycles==7){ Periodo = PERIOD_W1;  Minutes = 10080; }
      if (Cycles==8){ Periodo = PERIOD_MN1; Minutes = 43200; }
      
      // Calculations
      Range     = ((iHigh(NULL,Periodo,i+1) - iLow(NULL,Periodo,i+1))/100);
      Level_0   = iLow(NULL,Periodo,i+1);
      Level_25  = iLow(NULL,Periodo,i+1) + Range *25;
      Level_375 = iLow(NULL,Periodo,i+1) + Range*37.5;
      Level_50  = iLow(NULL,Periodo,i+1) + Range*50;
      Level_625 = iLow(NULL,Periodo,i+1) + Range*62.5;
      Level_75  = iLow(NULL,Periodo,i+1) + Range*75;
      Level_100 = iLow(NULL,Periodo,i+1) + Range*100;
      
      // Drawing the Lines
      Line_Start = iTime(NULL,Periodo,i);
      if (i==0){
         Line_End = iTime(NULL,Periodo,i)+(1*(Minutes*60));
         Draw_Label = true;
      }else{
         Line_End = iTime(NULL,Periodo,(i-1));
         Draw_Label = false;
      }
      
      Pivot("Level_0"+i,Line_Start,Level_0,Line_End, High_and_Low_Color,3, STYLE_SOLID,Draw_Label);
      Pivot("Level_25"+i,Line_Start,Level_25,Line_End, Level_25_and_75_Color,3, STYLE_SOLID,Draw_Label);
      Pivot("Level_375"+i,Line_Start,Level_375,Line_End, Level_37_5_and_62_5_Color,3, STYLE_SOLID,Draw_Label);
      Pivot("Level_50"+i,Line_Start,Level_50,Line_End, Level_50_Color,3, STYLE_SOLID,Draw_Label);
      Pivot("Level_625"+i,Line_Start,Level_625,Line_End, Level_37_5_and_62_5_Color,3, STYLE_SOLID,Draw_Label);
      Pivot("Level_75"+i,Line_Start,Level_75,Line_End, Level_25_and_75_Color,3, STYLE_SOLID,Draw_Label);
      Pivot("Level_100"+i,Line_Start,Level_100,Line_End, High_and_Low_Color,3, STYLE_SOLID,Draw_Label);
      
      if (Draw_Cycles_Separator) Separator("Sep"+i, Line_Start, Cycles_Separator_Color, 0, STYLE_DOT);
      
   }
   
   } // if Check==false
   
//----
   return(0);
}

void Pivot(string Nombre, datetime tiempo1, double precio1, datetime tiempo2, color bpcolor, int ancho, int style, bool draw_text){
   ObjectDelete(IndicatorObjPrefix + Nombre);
   ObjectCreate(IndicatorObjPrefix + Nombre, OBJ_TREND, 0, tiempo1, precio1, tiempo2, precio1);
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_COLOR, bpcolor);
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_STYLE, style);
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_WIDTH, ancho);
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_RAY, False);
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_BACK, true );
   if (Show_Labels && draw_text){
      ObjectDelete(IndicatorObjPrefix + "T"+Nombre);
      ObjectCreate(IndicatorObjPrefix + "T"+Nombre, OBJ_TEXT, 0, tiempo2+(2*Period()*60), precio1 );
      ObjectSetText(IndicatorObjPrefix + "T"+Nombre, StringSubstr(Nombre,0,StringLen(Nombre)-1), 10, "Arial", bpcolor );
      ObjectSet(IndicatorObjPrefix + "T"+Nombre, OBJPROP_TIME1, tiempo2+(2*Period()*60));
      ObjectSet(IndicatorObjPrefix + "T"+Nombre, OBJPROP_PRICE1, precio1);
   }
}

// Draw Separator
void Separator(string Nombre, datetime tiempo1, color sesscolor, int ancho, int style){
   ObjectDelete(IndicatorObjPrefix + Nombre);
   ObjectCreate(IndicatorObjPrefix + Nombre, OBJ_VLINE, 0, tiempo1, WindowPriceMax());
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_COLOR, sesscolor);
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_STYLE, style);
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_WIDTH, ancho);
   ObjectSet(IndicatorObjPrefix + Nombre, OBJPROP_BACK, True);
}

bool Check (){
   
   bool wrong_tf = false;
   
   if (Period()==5     && Cycles<2) wrong_tf = true;
   if (Period()==15    && Cycles<3) wrong_tf = true;
   if (Period()==30    && Cycles<4) wrong_tf = true;
   if (Period()==60    && Cycles<5) wrong_tf = true;
   if (Period()==240   && Cycles<6) wrong_tf = true;
   if (Period()==1440  && Cycles<7) wrong_tf = true;
   if (Period()==10080 && Cycles<8) wrong_tf = true;
   if (Period()==43200)             wrong_tf = true;
   
   return(wrong_tf);
   
}