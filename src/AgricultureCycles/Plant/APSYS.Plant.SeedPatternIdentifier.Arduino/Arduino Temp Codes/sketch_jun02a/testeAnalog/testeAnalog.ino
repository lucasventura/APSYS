int contador = 0;
int debounce = 0;

void setup() {
  // initialize serial communications at 9600 bps:
  Serial.begin(115200); 
  contador = 0;
}
void loop() 
{
 
  int sensor1 = analogRead(0);
  int sensor2 = analogRead(1);
  int sensor3 = analogRead(2);
  Serial.println(sensor1);
  Serial.println(sensor2);
  Serial.println(sensor3);
  Serial.println(); 
  delay(50);
  return;
  
  if(sensor1 > 300 || sensor2 > 300 || sensor3 > 300)
  {
     if(debounce == 0)
     {
      contador++;
      Serial.print("Contagem:");
      Serial.println(contador);
      Serial.println(sensor1);
      Serial.println(sensor2);
      Serial.println(sensor3);
      Serial.println();
     }  
        
      debounce++;    
  }
  else
  {
    debounce = 0;
  }       
  
  if(debounce > 1000)
  {
    Serial.print("Sensor Entupido.."); 
    Serial.println(debounce); 
  }
}
