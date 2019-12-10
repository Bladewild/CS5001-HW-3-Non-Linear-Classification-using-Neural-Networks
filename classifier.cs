using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
namespace HW2
{
    class Program
    {
        private static List<Coordinate> ListofData = new List<Coordinate>();
        private static List<Coordinate> ListofValidData = new List<Coordinate>();
        private static List<float> bestInputUnits = new List<float>();
        private static List<float> bestOutputUnits = new List<float>();
        private static float learningRate;
        public static int bestIteration;

        public static float bestSSE, bestETa, bestw0, bestw1;
        public static int numIterations = 5000;
        struct Coordinate
        {
            public int x;
            public int y;
            public int type;
            public Coordinate(int inputX, int inputY, int inputType)
            {
                x = inputX;
                y = inputY;
                type = inputType;
            }
            public Coordinate(string inputX, string inputY,string inputType)
            {
                x = Int32.Parse(inputX);
                y = Int32.Parse(inputY);
                type = Int32.Parse(inputType);
            }

            public Coordinate(string inputX, string inputY,string inputType)
            {
                x = Int32.Parse(inputX);
                y = Int32.Parse(inputY);
                type = Int32.Parse(inputType);
            }
            public override string ToString()
            {
                return "[" + x + "," + y + ","+type+"]";
            }
        }

        static void Main(string[] args)
        {
            float positiveInf = 1 / 0.0f; ;
            bestSSE = positiveInf;
            ReadFile();
            for (int i = 1; i <= 10000; i++)
            {
                LinearLearner(i);
                if (i % 100 == 0)
                {
                    Console.WriteLine("Iteration: " + i);
                }
            }

            Console.WriteLine("--------------");

            foreach (Coordinate c in ListofValidData)
            {
                float ycap = 0.0f;
                ycap += bestw0 * 1;
                ycap += bestw1 * c.x;
                float delta = (float)(c.y) - ycap;
                string str = c.y + " + " + ycap + " = " + delta;
                Console.WriteLine(str);
            }

            Console.WriteLine("--------------");
            Console.WriteLine("Input Units");
            foreach(float f in bestInputUnits)
            {
                Console.WriteLine(f);
            }
            Console.WriteLine("Output Units");
            foreach(float f in bestOutputUnits)
            {
                Console.WriteLine(f);
            }

            Console.WriteLine("bestsumofSquaresError: " + bestSSE);
            Console.WriteLine("bestLearningRate: " + bestETa);
            Console.WriteLine("bestIteration: " + bestIteration);

            //output
            StreamWriter file = new System.IO.StreamWriter("learner1output.txt");
            file.WriteLine("CS-5001: HW#1");
            file.WriteLine("Programmer: Alain Markus P. Santos-Tankia\n");
            file.WriteLine("Using learning rate eta = "+bestETa);
            file.WriteLine("Using "+numIterations+" iterations.\n");
            file.WriteLine("OUPUT");
            file.WriteLine("Validation");
            file.WriteLine("Sum-of-Squares Error = "+bestSSE);
            file.Close();

        }


        private static void ReadFile()
        {
            // Takinga a new input stream i.e.  
            // geeksforgeeks.txt and opens it 
            StreamReader sr = new StreamReader("hw3data.txt");

            // This is use to specify from where  
            // to start reading input stream 
            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            string[] input;

            

            while (sr.EndOfStream == false)
            {
                input = sr.ReadLine().Split('	');
                ListofData.Add(new Coordinate(input[0], input[1],input[2]));
            }

            // to close the stream 
            sr.Close();

            //
            sr = new StreamReader("hw3valid.txt");


            // This is use to specify from where  
            // to start reading input stream 
            sr.BaseStream.Seek(0, SeekOrigin.Begin);


            while (sr.EndOfStream == false)
            {
                input = sr.ReadLine().Split('	');
                ListofValidData.Add(new Coordinate(input[0], input[1],input[2]));
            }

            // to close the stream 
            sr.Close();

        }

        public static void LinearLearner(int iteration)
        {
            //PROCEDURE BackPropagation
            //   E  : Set of examples, each of the form ⟨ X, Y ⟩  where:
            //      X = ⟨ X1, X2, X3, ..., Xnx ⟩
            //      Y = ⟨ Y1, Y2, Y3, ..., Yny ⟩
            //   NN : A sequence of Layers

            //   Ycap[ ny ] : output of the Neural Network
            //learningRate = GenerateRandomFloat(0.0000000f, 0.00002f, 12);
            int numOutput= 1;
            int numInput= 3;
            layer[] neuralnetwork= createNeuralNetwork();
            float ycap;
            float delta;
            
           

            foreach(Coordinate e in Examples) //FOR EACH example e in E 
            {
                //in this case e's
                list<float> values = new list<float>(); //values := e.X
                values.add(e.x);
                values.add(e.y);
                for(int i=0;i<neuralnetwork.Length;i++) // FOR EACH layer L in NN, from input to output
                {
                    values= neuralnetwork[i].feedForward(values);
                }
                ycap = values[0];
                
                //   Ycap := values
                //   FOR EACH output j in [0..ny]
                //	 delta[j] := ( e.Yj - Ycap[y] ) * Ycap[y] * ( 1 - Ycap[y] )
                delta = (e.type-ycap)*ycap*(1-ycap)

                for(int i=0;i<neuralnetwork.Length;i++) // FOR EACH layer L in NN, from input to output
                {
                    delta = neuralnetwork[i].backProp(delta);
                }
            }

            //sum of squares

            //use validation
            float sumofSquaresError = 0.0f;
            foreach (Coordinate c in ListofValidData)
            {
                ycap = 0.0f;

                float delta = (float)(c.y) - ycap;
                sumofSquaresError += (float)Math.Pow(delta, 2);
            }
            
            //use validation
            if (sumofSquaresError < bestSSE)
            {
                bestIteration = iteration;
                
                bestSSE = sumofSquaresError;
                bestETa = learningRate;
            }

        }

        public static list<layer> createNeuralNetwork()
        {
            layer inputLayer = new layer(4,2);
            layer outputLayer= new layer(1,4);
            list<layer> networkModel= new list<layer>();
            networkModel.add(inputLayer);
            networkModel.add(outputLayer);
            return networkModel;
        }

    }

    class layer
    {
        struct unit
        {
            list<float> weightAt;
            list<float> weightChange;
            float weight;
            public unit(int numOutput,int numInput)
            {
                weightAt=new list<float(); //randomize weights
                startingWeight = GenerateRandomFloat(0.00f, 100.00f, 5);
                for(int i=0;i<numOutput;i++)
                {
                    weightAt.add(GenerateRandomFloat(0.00f, 100.00f, 5));
                }                          
                for(int i=0;i<numInput;i++)
                {
                    weightChange.add(0f);
                }  

            }
            
        }
        //nu : number of units
        //ni : number of inputs
        //W[ ni+1 ][ nu ] : Unit's weights // W[k][j] is weight applied to input k for unit j
        //input[ ni+1 ] : input vector
        //output[ nu ] : output vector
        int numberofUnits,numberofInputs;
        list<unit> Units;
        list<float> inputVector= new list<float>();
        list<float> outputVector= new list<float>();
        list<float> delta_prev=new list<float>;

        public layer(int inputNOFU,int inputNOFI)
        {            
            numberofUnits = inputNOFU;
            numberofInputs = inputNOFI;//might not be needed
            //create Units
            list<unit> Units;
            for(int i=0;i<numberofUnits;i++)    
            {
                Units.add(new unit(numberofUnits,numberofInputs));
            }
            for(int i=0;i<numberofInputs;i++)    
            {
                delta_prev.add(0f);
            }
        }
        static float feedForward(list<float> inp )//inp [ni]
        {
            //refresh/update vectors
            inputVector = inp; //input := inp;
            inputVector.add(1);//input[ ni ] := 1; extra special
            outputVector= new list<float>();

            int index=0;
            foreach(unit u in Units)//FOR every unit u in [ 0..nu ]
            {
                double value=0;
                //∑k=0ni W[ k ][ u ] * input[ k ]
                for(int k=0;k<inputVector.count;k++)//∑k=0ni
                {
                    value+=u.weightAt[k]*input[k];// W[ k ][ u ] * input[ k ]
                }
                outputVector.add(Sigmoid(value));
                index=++;
                //output[ u ] := sig ( ∑k=0ni W[ k ][ u ] * input[ k ] )
            }
            return = outputVector;
            //RETURN output;
        }


        //deltaW[ ni+1 ][ nu ] : Unit's weights change // W[k][j] is weight from input k to unit j
        //eta : Learning rate
        //alpha : momentum constant
        //delta_prev[ ni ] : error term of input

        static list<float> backProp(list<float> delta,float eta)//delta [nu]
        {
            float alpha=0.04f; //momentum constant change 0-1
            //FOR EACH input j in [0..ni]
            foreach(int j;j<inputVector.count;j++)
            {
                float newDeltaValue=0f;
                
                //delta_prev[ j ] := ( ∑u=0nu delta[ u ] * W[ j ][ u ] ) * input[ j ] * ( 1 - input[ j ] )
                for(int unit_no=0;unit_no<numberofUnits;unit_no++)//∑u=0nu
                {
                    // delta[ u ] * W[ j ][ u ] ) * input[ j ] * ( 1 - input[ j ]
                    newDeltaValue+=delta[unit_no] * W[j][unit_no]) * inputVector[j] * ( 1 - inputVector[j])
                }
                delta_prev[j]=newDeltaValue;
            }

            
            foreach(int j;j<inputVector.count;j++)//FOR EACH input j in [0..ni]
            {
                for(int unit_no=0;unit_no<numberofUnits;unit_no++) //FOR EACH unit u in [0..nu]
                {
                    //deltaW[ j ][ u ] := eta*delta[ u ]*output[ u ] + alpha*deltaW[ j ][ u ]
                    u.weightChange[j] := eta*delta[unit_no]*outputVector[unit_no] + alpha*u.weightChange[j];
                    //W[ j ][ u ] := W[ j ][ u ] + deltaW[ j ][ u ]
                    u.weightAt(j) := u.weightAt(j) + u.weightChange[j];
                }
            }
            return delta_prev;
            //RETURN delta_prev;
        }
        public static float Sigmoid(double value) {
            //sig ( ∑k=0ni W[ k ][ u ] * input[ k ] )
            return (float) (1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }

        public static float GenerateRandomFloat(float min, float max, int upto)
        {
            //convert max and min to decimal
            //find the decimal places, use that make max to be an int
            // mod that int to randomInteger, get result and convert back to float
            // add min to float

            decimal maxDecimal = (decimal)max;
            //Console.WriteLine("maxInteger: "+maxDecimal);

            //experiment later, for now set
            //int count = BitConverter.GetBytes(decimal.GetBits(maxDecimal)[3])[2];
            int count = upto;

            //Console.WriteLine("count: "+count);
            double powOf = Math.Pow(10f, count);
            int maxInteger = (int)(max * powOf);
            //Console.WriteLine("maxInteger: "+maxInteger);


            int randomInteger = Math.Abs(unchecked((int)GenerateRandomNumber()));


            int moddedRandom = randomInteger % maxInteger;
            float randomFloat = moddedRandom / ((float)powOf);

            float result = (randomFloat) + min;
            return result;
        }
        public static uint GenerateRandomNumber()
        {
            //Sourced from https://stackify.com/csharp-random-numbers/ 
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            //convert 4 bytes to an integer
            var byteArray = new byte[4];
            provider.GetBytes(byteArray);
            uint randomInteger = BitConverter.ToUInt32(byteArray, 0);
            return randomInteger;
        }

    }
}
