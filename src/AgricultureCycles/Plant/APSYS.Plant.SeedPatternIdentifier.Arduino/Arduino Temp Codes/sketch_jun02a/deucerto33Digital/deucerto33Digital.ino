int contador = 0;
int debounce = 0;

void setup() {
  // initialize serial communications at 9600 bps:
  Serial.begin(115200); 
  contador = 0;
}
void loop() 
{
 
  int sensor1 = digitalRead(5);
  int sensor2 = digitalRead(6);
  int sensor3 = digitalRead(7);
  
  if(sensor1 > 0 || sensor2 > 0 || sensor3 > 0)
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
