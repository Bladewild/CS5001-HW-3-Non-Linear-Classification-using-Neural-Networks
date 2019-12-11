using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
namespace HW2
{
    class Program
    {
        // TODO CHECK EQUATIONS
        //Delta equations value
        //is ycap only 1 output?
        //alpha constant?
        private static List<Coordinate> ListofData = new List<Coordinate>();
        private static List<Coordinate> ListofValidData = new List<Coordinate>();
        private static List<float> bestInputUnits = new List<float>();
        private static List<float> bestOutputUnits = new List<float>();
        private static float learningRate;
        public static int bestIteration,bestCorrect;

        public static float bestSSE, bestETa;
        public static int numIterations = 1000;
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
            for (int i = 1; i <= numIterations; i++)
            {
                NonLinearLearner(i);
                //if (i % 10 == 0)
                //{
                //    Console.WriteLine("Iteration: " + i);
                //}
            }

            Console.WriteLine("--------------");

            /*
            foreach (Coordinate c in ListofValidData)
            {
                float ycap = 0.0f;
                ycap += bestw0 * 1;
                ycap += bestw1 * c.x;
                float delta = (float)(c.y) - ycap;
                string str = c.y + " + " + ycap + " = " + delta;
                Console.WriteLine(str);
            }*/

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

            Console.WriteLine("bestCorrect: " + bestCorrect);
            Console.WriteLine("bestsumofSquaresError: " + bestSSE);
            Console.WriteLine("bestLearningRate: " + bestETa);
            Console.WriteLine("bestIteration: " + bestIteration);

            //output
            StreamWriter file = new System.IO.StreamWriter("'classifieroutput.txt");
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
                if(input[0] == "-----------------------------")
                    continue;

                ListofValidData.Add(new Coordinate(input[0], input[1],input[2]));
            }

            // to close the stream 
            sr.Close();

        }

        public static void NonLinearLearner(int iteration)
        {
            //PROCEDURE BackPropagation
            //   E  : Set of examples, each of the form ⟨ X, Y ⟩  where:
            //      X = ⟨ X1, X2, X3, ..., Xnx ⟩
            //      Y = ⟨ Y1, Y2, Y3, ..., Yny ⟩
            //   NN : A sequence of Layers

            //   Ycap[ ny ] : output of the Neural Network
            learningRate = cryptoRando.GenerateRandomFloat(0.00000000f, 100.00000000f, 8);
            int numOutput= 1;
            int numInput= 3;
            layer[] neuralnetwork= createNeuralNetwork();
            float ycap;
            
           
            string evostreak="";
            foreach(Coordinate e in ListofData) //FOR EACH example e in E 
            {
                //in this case e's
                List<float> values = new List<float>(); //values := e.X
                values.Add(e.x);
                values.Add(e.y);
                for(int i=0;i<neuralnetwork.Length;i++) // FOR EACH layer L in NN, from input to output
                {
                    values= neuralnetwork[i].feedForward(values);
                   //Console.WriteLine("--------");
                   //foreach(float v in values)
                   //{
                   //    Console.WriteLine(v);

                   //}
                   //Console.WriteLine("--------");
                }
                ycap = values[0];
                evostreak+= ycap.ToString();
                
                //   Ycap := values
                //   FOR EACH output j in [0..ny]
                //	 delta[j] := ( e.Yj - Ycap[y] ) * Ycap[y] * ( 1 - Ycap[y] )
                List<float> delta = new List<float>(); 
                //maybe change this if same or not
                //save float delta1 = (e.type-ycap)*ycap*(1f - ycap);\
                //Console.WriteLine(e.type +"="+ycap+"?");
                float delta1 = e.type-ycap;
                delta.Add(delta1);
                //Console.WriteLine("delta1: "+delta1);

                for(int i=neuralnetwork.Length-1;i>=0;i--) // FOR EACH layer L in NN, from output to input
                {
                    //remember to go output --> input
                    delta = neuralnetwork[i].backProp(delta,learningRate);
                }
            }
            Console.WriteLine("evostreak:"+evostreak);

            //sum of squares

            //use validation
            float sumofSquaresError = 0.0f;
            int correct=0;
            Console.WriteLine("Guessing");
            foreach (Coordinate e in ListofValidData)
            {
                List<float> values = new List<float>(); //values := e.X
                values.Add(e.x);
                values.Add(e.y);
                
                for(int i=0;i<neuralnetwork.Length;i++) // FOR EACH layer L in NN, from input to output
                {
                    values= neuralnetwork[i].feedForward(values);
                }
                Console.Write(values[0]);
            
                float delta = (float)(e.type) - values[0];
                if(e.type==values[0])
                    correct++;
                sumofSquaresError += (float)Math.Pow(delta, 2);
            }
            Console.WriteLine();
            Console.WriteLine("---");
            Console.WriteLine("Iteration: "+iteration);
            Console.WriteLine(sumofSquaresError);
            Console.WriteLine(correct);            
            Console.WriteLine(learningRate);
            /*
            Console.WriteLine("INPUT");
            foreach(unit u in neuralnetwork[0].Units)
            {
                foreach(float input in u.weightAt)
                    Console.WriteLine(input);
            }
            Console.WriteLine("OUTPUT");
            
            bestOutputUnits= new List<float>();
            
            foreach(unit u in neuralnetwork[1].Units)
            {
                foreach(float input in u.weightAt)
                    Console.WriteLine(input);
            }
            */
            Console.WriteLine("---------------------");
            
            //use validation
            if (sumofSquaresError < bestSSE)
            {
                bestIteration = iteration;
                bestInputUnits= new List<float>();
                bestCorrect = correct;
                foreach(unit u in neuralnetwork[0].Units)
                {
                    foreach(float input in u.weightAt)
                        bestInputUnits.Add(input);
                }
                
                bestOutputUnits= new List<float>();
                
                foreach(unit u in neuralnetwork[1].Units)
                {
                    foreach(float input in u.weightAt)
                        bestOutputUnits.Add(input);
                }
                
                bestSSE = sumofSquaresError;
                bestETa = learningRate;
            }
        }

        public static layer[] createNeuralNetwork()
        {
            layer inputLayer = new layer(4,2,"input");
            layer outputLayer= new layer(1,4,"output");
            List<layer> networkModel= new List<layer>();
            networkModel.Add(inputLayer);
            networkModel.Add(outputLayer);
            return networkModel.ToArray();
        }

        

    }

    class layer
    {
        

        //nu : number of units
        //ni : number of inputs
        //W[ ni+1 ][ nu ] : Unit's weights // W[k][j] is weight applied to input k for unit j
        //input[ ni+1 ] : input vector
        //output[ nu ] : output vector
        int numberofUnits,numberofInputs;
        public List<unit> Units;
        public List<float> inputVector= new List<float>();
        public List<float> outputVector= new List<float>();
        List<float> delta_prev=new List<float>();
        public string name;

        float alpha;

        public layer(int inputNOFU,int inputNOFI,string inputName)
        {            
            
            alpha=cryptoRando.GenerateRandomFloat(0.0000000f, 1.0000000f, 7); //momentum constant change 0-1
            name= inputName;
            numberofUnits = inputNOFU;
            numberofInputs = inputNOFI;//might not be needed
            //create Units
            Units=new List<unit>();
            for(int i=0;i<numberofUnits;i++)    
            {
                Units.Add(new unit(numberofUnits,numberofInputs));
            }
            for(int i=0;i<numberofInputs;i++)    
            {
                delta_prev.Add(0f);
            }
        }
        public List<float> feedForward(List<float> inp )//inp [ni]
        {
            //refresh/update vectors
            inputVector = inp; //input := inp;
            /*
            Console.WriteLine("feedforward: "+name);
            inputVector.Add(1);//input[ ni ] := 1; extra special
            foreach(float f in inputVector)
            {
                Console.WriteLine(f);
            }
            Console.WriteLine("feedforward2");
            */
            outputVector= new List<float>();

            int index=0;
            foreach(unit u in Units)//FOR every unit u in [ 0..nu ]
            {
                double value=0;
                //∑k=0ni W[ k ][ u ] * input[ k ]
                for(int k=0;k<numberofInputs;k++)//∑k=0ni
                {
                    value+=u.weightAt[k]*inputVector[k];// W[ k ][ u ] * input[ k ]
                }
                outputVector.Add(Sigmoid(value));
                index++;
                //output[ u ] := sig ( ∑k=0ni W[ k ][ u ] * input[ k ] )
            }
            return outputVector;
            //RETURN output;
        }


        //deltaW[ ni+1 ][ nu ] : Unit's weights change // W[k][j] is weight from input k to unit j
        //eta : Learning rate
        //alpha : momentum constant
        //delta_prev[ ni ] : error term of input

        public List<float> backProp(List<float> delta,float eta)//delta [nu]
        {
            //FOR EACH input j in [0..ni]
            for(int j=0;j<numberofInputs;j++)
            {
                float newDeltaValue=0f;
                
                //delta_prev[ j ] := ( ∑u=0nu delta[ u ] * W[ j ][ u ] ) * input[ j ] * ( 1 - input[ j ] )
                for(int unit_no=0;unit_no<numberofUnits;unit_no++)//∑u=0nu
                {
                    // delta[ u ] * W[ j ][ u ] ) * input[ j ] * ( 1 - input[ j ]
                    //newDeltaValue+= delta[unit_no] * Units[unit_no].weightAt[j] * inputVector[j] * ( 1 - inputVector[j]); // TODO CHECK EQUATIONS
                    //Console.WriteLine(delta[unit_no] +","+ Units[unit_no].weightAt[j] +","+ inputVector[j]);
                    newDeltaValue+= delta[unit_no] * Units[unit_no].weightAt[j] * inputVector[j]; //* ( 1 - inputVector[j]); // TODO CHECK EQUATIONS
                }
                delta_prev[j]=newDeltaValue;
            }

            
            //Console.WriteLine("|--|");
            for(int j=0;j<numberofInputs;j++)//FOR EACH input j in [0..ni]
            {
                for(int unit_no=0;unit_no<numberofUnits;unit_no++) //FOR EACH unit u in [0..nu]
                {
                    //deltaW[ j ][ u ] := eta*delta[ u ]*output[ u ] + alpha*deltaW[ j ][ u ]
                    //Console.WriteLine(eta*delta[unit_no] +","+ outputVector[unit_no] +","+ alpha*Units[unit_no].weightChange[j]);
                    Units[unit_no].weightChange[j] = eta*delta[unit_no]*outputVector[unit_no] + alpha*Units[unit_no].weightChange[j];
                    //Console.WriteLine( Units[unit_no].weightChange[j]);
                    //W[ j ][ u ] := W[ j ][ u ] + deltaW[ j ][ u ]
                    Units[unit_no].weightAt[j] = Units[unit_no].weightAt[j] + Units[unit_no].weightChange[j];
                }
            }
            //Console.WriteLine("||");
            return delta_prev;
            //RETURN delta_prev;
        }
        public static float Sigmoid(double value) {
            //sig ( ∑k=0ni W[ k ][ u ] * input[ k ] )
            return (float) (1.0 / (1.0 + Math.Exp(-value)));
        }

        

    }

    public struct unit
    {
        public List<float> weightAt;
        public List<float> weightChange;
        public unit(int numOutput,int numInput)
        {
            weightAt=new List<float>(); //randomize weights
            weightChange=new List<float>(); //randomize weights
            for(int i=0;i<numInput+1;i++)
            {
                weightAt.Add(cryptoRando.GenerateRandomFloat(0.00f, 100.00f, 5));
            }                          
            for(int i=0;i<numInput+1;i++)
            {
                weightChange.Add(0f);
            }  
        }            
    }

    class cryptoRando
    {
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
            return BitConverter.ToUInt32(byteArray, 0);
        }
    }
}
