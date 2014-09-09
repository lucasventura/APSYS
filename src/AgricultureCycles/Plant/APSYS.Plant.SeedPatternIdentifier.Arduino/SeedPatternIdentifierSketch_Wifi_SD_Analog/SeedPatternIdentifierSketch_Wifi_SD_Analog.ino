#include <SPI.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include <SD.h>

int status = WL_IDLE_STATUS;
char ssid[] = "SURIA"; //  your network SSID (name) 
char pass[] = "MadonaBoliSuriaMax";    // your network password (use for WPA, or use as key for WEP)
int keyIndex = 0;            // your network key Index number (needed only for WEP)

unsigned int localPort = 2390;      // local port to listen on
char packetBuffer[255]; //buffer to hold incoming packet
WiFiUDP Udp;

int sensorNumber = 4;
int reads[7] = {0,0,0,0,0,0,0};
int cont;
int limiar = 100;

const int chipSelect = 4;
char* fileName = "datalog.txt";

void setup() {
  pinMode(10, OUTPUT);
  pinMode(4, OUTPUT);
  digitalWrite(10, HIGH);
  digitalWrite(4, HIGH);

  //Initialize serial and wait for port to open:
  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }
  
  Calibrate();
  
  // check for the presence of the shield:
  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WiFi shield not present");
    // don't continue:
    while (true);
  }

  String fv = WiFi.firmwareVersion();
  if ( fv != "1.1.0" )
    Serial.println("Please upgrade the firmware");

  Serial.print("Initializing SD card...");  
  // see if the card is present and can be initialized:
  if (!SD.begin(chipSelect)) {
    Serial.println("Card failed, or not present");
    // don't do anything more:
    return;
  }
  Serial.println("card initialized.");
  
   File myFile = SD.open(fileName, FILE_WRITE);
  if(myFile)
  {
    myFile.println("Iniciando");
  }    
  myFile.close();
  
  // attempt to connect to Wifi network:
  while ( status != WL_CONNECTED) {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    status = WiFi.begin(ssid,pass);
    
    delay(2000);
  }
  Serial.println("Connected to wifi");
  printWifiStatus();

  Serial.println("\nStarting connection to server...");
  // if you get a connection, report back via serial:
  Udp.begin(localPort);
  
  // Calibrate();
}

void loop() {
  String onlyWithSeed;
  String data;
  // if there's data available, read a packet
  int packetSize = Udp.parsePacket();
  if (packetSize)
  {
    // read the packet into packetBufffer
    int len = Udp.read(packetBuffer, 255);
    if (len > 0)
	{
		String receiveData = packetBuffer;
                if(receiveData.startsWith("L"))
                {
                  limiar = receiveData.substring(1).toInt();                  
                  Serial.println(packetBuffer);
  		  Serial.print("limiar");
		  Serial.println(limiar); 
                }
                else if(receiveData.startsWith("C"))
                {
                  Serial.println("Zerado");
                  cont=0;
                }
                else if(receiveData.startsWith("K"))
                {
                  Calibrate();
                }
                
		 
		packetBuffer[len] = 0;
	}	
  }
  
  File dataFile = SD.open(fileName, FILE_WRITE);
   boolean seed = false;	
	data = "$";	
	for (int analogChannel = 0; analogChannel < sensorNumber; analogChannel++)
	{
                String tempString;
		int sensorReading = analogRead(analogChannel);
		tempString =  String(analogChannel) + "," + String(sensorReading) + "_";	
                data += tempString;		
                if(sensorReading > reads[analogChannel])
		{
                  onlyWithSeed +=tempString;
		  seed = true;
		}				
		// reads[analogChannel] = sensorReading;	
	}
	data += ";\r\n";

        int tam = data.length() + 1;
	char cstr[tam];
	data.toCharArray(cstr, tam);   
        Udp.beginPacket(Udp.remoteIP(), localPort);        
	Udp.write(cstr);  
        if (dataFile) 
        {
          dataFile.println(data);
        }  
        else 
        {
          // Serial.println("error opening d001.txt");
        }        
        dataFile.close();               
	Udp.endPacket();    

        if(seed)
	{
          digitalWrite(13,HIGH);
          digitalWrite(13,LOW);
          cont++;          
          Serial.print(cont);          
          Serial.print(" ------ "); 	
          Serial.println(onlyWithSeed);      
	}
}


void printWifiStatus() {
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");
}

void Calibrate()
{
  Serial.println("\nCalibrando...");
  for (int analogChannel = 0; analogChannel < sensorNumber; analogChannel++)
  {                
    int sensorReading = analogRead(analogChannel);
    reads[analogChannel] = sensorReading * 1.1;	
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




