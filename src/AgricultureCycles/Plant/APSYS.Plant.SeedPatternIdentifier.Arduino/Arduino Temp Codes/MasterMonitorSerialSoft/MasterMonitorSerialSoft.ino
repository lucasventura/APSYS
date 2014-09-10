#include <SPI.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include <SD.h>
#include <SoftwareSerial.h>

int status = WL_IDLE_STATUS;
char ssid[] = "SURIA"; //  your network SSID (name) 
char pass[] = "MadonaBoliSuriaMax";    // your network password (use for WPA, or use as key for WEP)
int keyIndex = 0;            // your network key Index number (needed only for WEP)

unsigned int localPort = 2390;      // local port to listen on
char packetBuffer[255]; //buffer to hold incoming packet
WiFiUDP Udp;

int sensorNumber = 4;
int cont;
int limiar = 100;

const int chipSelect = 4;
char* fileName = "datalog.txt";

SoftwareSerial port1(12,13);
//SoftwareSerial port2(63,17);
//SoftwareSerial port3(64,19);
//SoftwareSerial port4(13,21);

void setup() {
  pinMode(10, OUTPUT);
  pinMode(4, OUTPUT);
  digitalWrite(10, HIGH);
  digitalWrite(4, HIGH);

  //Initialize serial and wait for port to open:
  Serial.begin(9600);
  Serial1.begin(9600);
  Serial2.begin(9600);
  Serial3.begin(9600);
  port1.begin(9600);
//  port2.begin(9600);
//  port3.begin(9600);
//  port4.begin(9600);
    
  // Calibrate();
  
  // check for the presence of the shield:
  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WiFi shield not present");
    // don't continue:
    // while (true);
    return;
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
  String data = "$";	
  
  if(Serial1.available() > 0)
  {
    data += Serial1.readStringUntil('\n');
  }
   if(Serial2.available() > 0)
  {
    data += Serial2.readStringUntil('\n');
  }
   if(Serial3.available() > 0)
  {
    data += Serial3.readStringUntil('\n');
  }
  
  port1.listen();
  if(port1.available() > 0)
  {
    data += port1.readStringUntil('\n');
  }

  if(data.length() > 1)
  {
        int tam = data.length() + 1;
	char cstr[tam];
	data.toCharArray(cstr, tam);   
        Udp.beginPacket(Udp.remoteIP(), localPort);        
	Udp.write(cstr);  
        Udp.endPacket();
        
        File dataFile = SD.open(fileName, FILE_WRITE);  
        if (dataFile) 
        {
          dataFile.println(data);
        }  
        else 
        {
          Serial.println("error opening d001.txt");
        }        
        dataFile.close();               
	
        Serial.println(data);
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




