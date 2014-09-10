#include <SoftwareSerial.h>

int sensorNumber = 8;
int reads[8] = {0,0,0,0,0,0,0,0};
int cont;
int cont1;
int limiar = 100;
boolean debounce;
SoftwareSerial port1(3,4);

void setup() 
{
	Serial.begin(9600);
	port1.begin(9600);
	Calibrate();  
        debounce = false;
}

void loop() 
{
        String onlyWithSeed;
	String data = "$";	  
	boolean seed = false;	
        boolean seed1 = false;	
	
	for (int analogChannel = 0; analogChannel < sensorNumber; analogChannel++)
	{
		String tempString;
		int sensorReading = analogRead(analogChannel);
		tempString =  String(analogChannel) + "," + String(sensorReading) + "_";	
		data += tempString;		
		if(sensorReading > reads[analogChannel])
		{
			onlyWithSeed +=tempString;
                        if(analogChannel > 3)
                        {                          
  			  seed1 = true;
                        }
                        else
                        {
                          seed = true;
                        }
		}				
		// reads[analogChannel] = sensorReading;	
	}
	
	data += ";\r\n";       

	if(seed || seed1)
	{
          if(debounce)
          {
            
            digitalWrite(12,LOW); 
            digitalWrite(13,LOW);
            // Serial.println("debounce");
          }
          else
          {
                digitalWrite(13,HIGH);  
                digitalWrite(12,HIGH);
                if(seed1)
                {
                  cont1++;
                }
                if(seed)
                {
                  cont++;
                }
                          
		port1.print(cont); 
                port1.print(" -- "); 
                port1.print(cont1); 
                port1.print(" -- ");
                port1.println(onlyWithSeed);
                Serial.print(cont); 
                Serial.print(" -- "); 
                Serial.print(cont1); 
                Serial.print(" -- ");
		Serial.print(onlyWithSeed);   
                Serial.print(" ------ "); 	
		Serial.println(data);   
                debounce = true;
          }
	}
        else
        {
          digitalWrite(12,LOW); 
          digitalWrite(13,LOW); 	
          debounce = false;
        }        
}


void Calibrate()
{
	Serial.println("\nCalibrando...");
	
	for (int analogChannel = 0; analogChannel < sensorNumber; analogChannel++)
	{               
            int sensorReading = analogRead(analogChannel);
            if(analogChannel > 4)
            { 
                sensorReading = 1023;
            }
            
            reads[analogChannel] = sensorReading * 1.15;
               
               Serial.print("Sensor ");
	        Serial.print(analogChannel);
		Serial.print(" Tolerancia ");
		Serial.print(sensorReading);
		Serial.print("\tAtribuido ");
		Serial.print(reads[analogChannel]);
		Serial.println("...");
	}
	
	Serial.println("\nCalibrado...");
}




