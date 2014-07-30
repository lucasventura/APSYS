void setup()
{
  // initialize serial communications at 9600 bps:
  Serial.begin(115200);   
  Serial.println("Iniciando");
}

void loop() 
{
  int sensor1 = analogRead(0);
  int sensor2 = analogRead(1);
  int sensor3 = analogRead(2);
  
  Serial.print("$");
  Serial.print("1");
  Serial.print(",");
  Serial.print(sensor1);
  Serial.print("_");
  
  //Serial.print("$");
  Serial.print("2");
  Serial.print(",");
  Serial.print(sensor2);
  Serial.print("_");
  
  //Serial.print("$");
  Serial.print("3");
  Serial.print(",");
  Serial.print(sensor3);
  Serial.println(";");
}
