
#include <SPI.h>
#include <WiFi.h>


char ssid[] = "SURIA"; //  your network SSID (name) 
char pass[] = "MadonaBoliSuriaMax";    // your network password (use for WPA, or use as key for WEP)
int keyIndex = 0;                 // your network key Index number (needed only for WEP)

int status = WL_IDLE_STATUS;

WiFiServer server(80);

// Enabe debug tracing to Serial port.
#define DEBUGGING

// Here we define a maximum framelength to 64 bytes. Default is 256.
#define MAX_FRAME_LENGTH 64

// Define how many callback functions you have. Default is 1.
#define CALLBACK_FUNCTIONS 1  

#include <WebSocketServer.h>

WebSocketServer webSocketServer;


// Called when a new message from the WebSocket is received
// Looks for a message in this form:
//
// DPV
//
// Where: 
//        D is either 'd' or 'a' - digital or analog
//        P is a pin #
//        V is the value to apply to the pin
//

void handleClientData(String &dataString) {
  bool isDigital = dataString[0] == 'd';
  int pin = dataString[1] - '0';
  int value;

  value = dataString[2] - '0';

    
  pinMode(pin, OUTPUT);
   
  if (isDigital) {
    digitalWrite(pin, value);
  } else {
    analogWrite(pin, value);
  }
    
  Serial.println(dataString);
}

// send the client the analog value of a pin
void sendClientData(int pin) {
  String data = "a";
  
  pinMode(pin, INPUT);
  data += String(pin) + String(analogRead(pin));
  webSocketServer.sendData(data);  
}

void setup() {
  //Initialize serial and wait for port to open:
  Serial.begin(9600); 
  while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }
  
  // check for the presence of the shield:
  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WiFi shield not present"); 
    // don't continue:
    while(true);
  } 
  
  // attempt to connect to Wifi network:
  while ( status != WL_CONNECTED) { 
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:    
    status = WiFi.begin(ssid, pass);

    // wait 10 seconds for connection:
    delay(2000);
  } 
  server.begin();
  // you're connected now, so print out the status:
  printWifiStatus();
}

void loop() {
  String data;
  WiFiClient client = server.available();
  
  if (client.connected() && webSocketServer.handshake(client)) {
    
    while (client.connected()) 
    {
      data = webSocketServer.getData();

      if (data.length() > 0) 
      {
        Serial.println(data);
        handleClientData(data);
      }
    }
  }
  
  // wait to fully let the client disconnect
  delay(50);
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
