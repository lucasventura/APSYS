void setup()
{
  // initialize serial communications at 9600 bps:
  Serial.begin(115200);   
}

void loop() 
{
  int sensor1 = analogRead(0);
  int sensor2 = analogRead(1);
  int sensor3 = analogRead(2);
  
  Serial.print("1");
  Serial.print(";");
  Serial.print(sensor1);
  Serial.println(";");
  
  Serial.print("2");
  Serial.print(";");
  Serial.print(sensor2);
  Serial.println(";");
  
  Serial.print("3");
  Serial.print(";");
  Serial.print(sensor3);
  Serial.println(";");
}
