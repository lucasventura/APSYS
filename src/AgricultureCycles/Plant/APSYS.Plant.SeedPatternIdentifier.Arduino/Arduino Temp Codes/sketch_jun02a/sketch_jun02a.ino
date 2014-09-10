int contador = 0;
int debounce = 0;

void setup() {
  // initialize serial communications at 9600 bps:
  Serial.begin(115200); 
}
void loop() 
{
  int soma = 0;
  
   soma += (digitalRead(5)) * (pow(2,0));
	 soma += (digitalRead(6)) * (pow(2,1));
	 soma += (digitalRead(7)) * (pow(2,2));
//	 soma += (digitalRead(6)) * (pow(2,3));
//	 soma += (digitalRead(11)) * (pow(2,4));
//	 soma += (digitalRead(7)) * (pow(2,5));
//	 soma += (digitalRead(12)) * (pow(2,6));
//	 soma += (digitalRead(8)) * (pow(2,7));
//	 soma += (digitalRead(13)) * (pow(2,8));

//	Serial.print(soma);
//        Serial.print("--- ");
//        Serial.print(digitalRead(9));
//	Serial.print(digitalRead(5));
//	Serial.print(digitalRead(10));
//	Serial.print(digitalRead(6));
//	Serial.print(digitalRead(11));
//	Serial.print(digitalRead(7));
//	Serial.print(digitalRead(12));
//	Serial.print(digitalRead(8));
//	Serial.println(digitalRead(13));

        if(soma > 0)
        {
          if(debounce == 0)
          {
            contador++;
          }  
          
          debounce++;    
        }
        else
        {
          debounce = 0;
        }       
        
        if(debounce > 100)
        {
          Serial.print("Sensor Entupido.."); 
          Serial.println(debounce); 
        }
        else
        {        
        Serial.print("Soma: ");
        Serial.print(soma);
        Serial.print(" --- Numero de Sementes: ");
        Serial.println(contador);
        }     
}
