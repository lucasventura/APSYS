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

int cont;

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
  // make sure that the default chip select pin is set to
  // output, even if you don't use it:
  // pinMode(10, OUTPUT);
  
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
//  if (SD.exists("d002.txt")) 
//  {
//    Serial.println("d002.txt exists.");
//  }
//  else 
//  {
//    Serial.println("d002.txt doesn't exist.");
//    // open a new file and immediately close it:
//    Serial.println("Creating d002.txt...");
//    File myFile = SD.open("d002.txt", FILE_WRITE);
//    myFile.close();
//  }  
  
  // attempt to connect to Wifi network:
  while ( status != WL_CONNECTED) {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    status = WiFi.begin(ssid,pass);

    // wait 10 seconds for connection:
    delay(2000);
  }
  Serial.println("Connected to wifi");
  printWifiStatus();

  Serial.println("\nStarting connection to server...");
  // if you get a connection, report back via serial:
  Udp.begin(localPort);
}

void loop() {
  String data;
  // if there's data available, read a packet
  int packetSize = Udp.parsePacket();
  File dataFile = SD.open(fileName, FILE_WRITE);
//  Serial.print("packetSize: ");
//  Serial.println(packetSize);
  if (packetSize)
  {
    // read the packet into packetBufffer
    int len = Udp.read(packetBuffer, 255);
    if (len > 0) packetBuffer[len] = 0;
  }
  
  data = "$";	
	/* for (int analogChannel = 0; analogChannel < 5; analogChannel++)
	{
		int sensorReading = analogRead(analogChannel);
		data +=  String(analogChannel) + "," + String(sensorReading) + "_";			
	} */
	
    boolean seed = false;
    	
    int pins [5] = { 2, 3, 5, 6, 8 };
    int reads [5] = {0,0,0,0,0};
	
	for (int i = 0; i < 5; i++)
	{
		int digitalChannel = pins[i];                
		reads[i] = digitalRead(digitalChannel);							
	} 
    
	for (int i = 0; i < 5; i++)
	{
        int sensorReading = reads[i];
		data +=  String(i+1) + "," + String(sensorReading) + "_";
		if(sensorReading > 0)
		{
			seed = true;
			//break;
		}						
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
          Serial.println("error opening d001.txt");
        }        
        dataFile.close();               
	Udp.endPacket();    

	if(seed)
	{
	  cont++;          
	  Serial.print(cont);          
	  Serial.print(" ------ "); 	
	  Serial.print(cstr);      
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




