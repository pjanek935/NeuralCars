using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class NeuralNetworkBackProp
    {
        //Struktura sieci
        private int inputNeuronsCount;
        private int hiddenNeuronsCount;
        private int outputNeuronsCount;

        //Wagi sieci
        private double[][] input2hiddenWeights; //Wagi połączeń warstwy wejściowej do ukrytej
        private double[][] hidden2outputWeights; //ukrytej do wyjściowej
        private double[] hiddenBiases; //Biasy warstwy ukrytej
        private double[] outputBiases; //warstwy wyjściowej

        //Wartości
        private double[] input;
        private double[] hiddenOutputs;
        private double[] outputs;

        //Zmienna rand do inicjalizacji wag
        private Random rnd;

        public NeuralNetworkBackProp(int inputNeuronsCount, int hiddenNeuronsCount, int outputNeuronsCount)
        {
            //Inicjalizacja struktury sieci
            this.inputNeuronsCount = inputNeuronsCount;
            this.hiddenNeuronsCount = hiddenNeuronsCount;
            this.outputNeuronsCount = outputNeuronsCount;

            //Inicjalizacja tablic wag
            input2hiddenWeights = MakeArray(inputNeuronsCount, hiddenNeuronsCount);
            hidden2outputWeights = MakeArray(hiddenNeuronsCount, outputNeuronsCount);
            hiddenBiases = new double[hiddenNeuronsCount];
            outputBiases = new double[outputNeuronsCount];

            input = new double[inputNeuronsCount];
            hiddenOutputs = new double[hiddenNeuronsCount];
            outputs = new double[outputNeuronsCount];

            rnd = new Random((int)DateTime.Now.Ticks);

            InitWeights(); //Przypisanie losowych wag sieci
        } // ctor

        private static double[][] MakeArray(int rows, int cols)
        {
            double[][] arr = new double[rows][];
            for (int row = 0; row < arr.Length; ++row)
            {
                arr[row] = new double[cols];
            }
            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    arr[row][col] = 0.0;
                }
            }
            return arr;
        }

        public void InitWeights()
        {
            double[] initialWeights = GetRandomWeights();
            SetWeights(initialWeights);
        }

        public double[] GetRandomWeights()
        {
            int weightsCount = (inputNeuronsCount * hiddenNeuronsCount) +
              (hiddenNeuronsCount * outputNeuronsCount) + hiddenNeuronsCount + outputNeuronsCount;
            double[] randomWeights = new double[weightsCount];
            for (int i = 0; i < randomWeights.Length; ++i)
            {
                randomWeights[i] = rnd.NextDouble() * 2 - 1f;
            }
            return randomWeights;
        }

        public void SetWeights(double[] weights)
        {
            int weightsCount = (inputNeuronsCount * hiddenNeuronsCount) +
              (hiddenNeuronsCount * outputNeuronsCount) + hiddenNeuronsCount + outputNeuronsCount;
            if (weights.Length != weightsCount)
            {
                throw new Exception("Bad weights array in SetWeights");
            }

            int weightsIndex = 0; //Zmienna pomocnicza reprezentująca aktualnie iterowany index
                                  //tablicy weights

            //Wagi połączeń warstwy wejściowej do ukrytej
            for (int i = 0; i < inputNeuronsCount; ++i)
            {
                for (int j = 0; j < hiddenNeuronsCount; ++j)
                {
                    input2hiddenWeights[i][j] = weights[weightsIndex];
                    weightsIndex++;
                }
            }

            //Biasy warstwy ukrytej
            for (int i = 0; i < hiddenNeuronsCount; ++i)
            {
                hiddenBiases[i] = weights[weightsIndex];
                weightsIndex++;
            }

            //Wagi połączeń warstwy ukrytej do wejściowej
            for (int i = 0; i < hiddenNeuronsCount; ++i)
            {
                for (int j = 0; j < outputNeuronsCount; ++j)
                {
                    hidden2outputWeights[i][j] = weights[weightsIndex];
                    weightsIndex++;
                }
            }

            //Biasy warstwy wyjściowej
            for (int i = 0; i < outputNeuronsCount; ++i)
            {
                outputBiases[i] = weights[weightsIndex];
                weightsIndex++;
            }
        }

        public double[] GetWeights()
        {
            int weightsCount = (inputNeuronsCount * hiddenNeuronsCount) +
              (hiddenNeuronsCount * outputNeuronsCount) + hiddenNeuronsCount + outputNeuronsCount;
            double[] weights = new double[weightsCount];

            int weightsIndex = 0; //Zmienna pomocnicza reprezentująca aktualnie iterowany index
                                  //tablicy weights

            //Wagi połączeń warstwy wejściowej do ukrytej
            for (int i = 0; i < input2hiddenWeights.Length; ++i)
            {
                for (int j = 0; j < input2hiddenWeights[0].Length; ++j)
                {
                    weights[weightsIndex] = input2hiddenWeights[i][j];
                    weightsIndex++;
                }
            }

            //Biasy warstwy ukrytej
            for (int i = 0; i < hiddenBiases.Length; ++i)
            {
                weights[weightsIndex] = hiddenBiases[i];
                weightsIndex++;
            }

            //Wagi połączeń warstwy ukrytej do wejściowej
            for (int i = 0; i < hidden2outputWeights.Length; ++i)
            {
                for (int j = 0; j < hidden2outputWeights[0].Length; ++j)
                {
                    weights[weightsIndex] = hidden2outputWeights[i][j];
                    weightsIndex++;
                }
            }

            //Biasy warstwy wyjściowej
            for (int i = 0; i < outputBiases.Length; ++i)
            {
                weights[weightsIndex] = outputBiases[i];
                weightsIndex++;
            }

            return weights;
        }

        public double[] GetOutput(double[] input)
        {
            double[] hiddenSums = new double[hiddenNeuronsCount]; //Sumy wyników z węzłów warswty ukrytej
            double[] outputSums = new double[outputNeuronsCount]; //Sumy wyników z węzłów warstwy wyjściowej

            //Skopiowanie wartości wejściowych do globalnej tablicy
            for (int i = 0; i < input.Length; ++i)
            {
                this.input[i] = input[i];
            }
                
            //Obliczenie wartości na wyjściach warstwy ukrytej
            for (int j = 0; j < hiddenNeuronsCount; ++j)
            {
                for (int i = 0; i < inputNeuronsCount; ++i)
                {
                    hiddenSums[j] += input[i] * input2hiddenWeights[i][j];
                }
            }

            //Dodanie biasów do wyników z warstwy ukrytej
            for (int i = 0; i < hiddenNeuronsCount; ++i)
            {
                hiddenSums[i] += hiddenBiases[i];
            }

            //Zastosowanie funkcji aktywacji dla wyników z warstwy ukrytej
            for (int i = 0; i < hiddenNeuronsCount; ++i)
            {
                hiddenOutputs[i] = HyperTan(hiddenSums[i]);
            }

            //Obliczenie wartości na wyjściach warstwy wyjściowej
            for (int j = 0; j < outputNeuronsCount; ++j)
            {
                for (int i = 0; i < hiddenNeuronsCount; ++i)
                {
                    outputSums[j] += hiddenOutputs[i] * hidden2outputWeights[i][j];
                }
            }

            //Dodanie biasów do wyników z warswty wyjściowej
            for (int i = 0; i < outputNeuronsCount; ++i)
            {
                outputSums[i] += outputBiases[i];
            }

            //Zastosowanie funkcji soft max
            double[] softOut = Softmax(outputSums);
            Array.Copy(softOut, outputs, softOut.Length);

            double[] retResult = new double[outputNeuronsCount];
            Array.Copy(outputs, retResult, retResult.Length);
            return retResult;
        }

        private static double HyperTan(double x)
        {
            if (x < -20.0) return -1.0;
            else if (x > 20.0) return 1.0;
            else return Math.Tanh(x);
        }

        private static double[] Softmax(double[] outputSums)
        {
            //Funkcja skaluje wartości wyjściowe tak aby
            //sumowały się do 1
            double sum = 0.0;
            for (int i = 0; i < outputSums.Length; ++i)
            {
                sum += Math.Exp(outputSums[i]);
            }
            
            double[] result = new double[outputSums.Length];
            for (int i = 0; i < outputSums.Length; ++i)
            {
                result[i] = Math.Exp(outputSums[i]) / sum;
            }
          
            return result;
        }

        public void BackProp(double[][] trainData, int maxEpochs,
          double learnRate, double momentum)
        {

            //Tablice gradientów
            double[][] input2hiddenGradients = MakeArray(inputNeuronsCount, hiddenNeuronsCount);  
            double[][] hidden2outputGradients = MakeArray(hiddenNeuronsCount, outputNeuronsCount);
            double[] outputBiasGradients = new double[outputNeuronsCount];                   
            double[] hiddenBiasGradients = new double[hiddenNeuronsCount];        

            //Lokalne sygnały dla wstecznej propagacji
            double[] outputSignals = new double[outputNeuronsCount];
            double[] hiddenSignals = new double[hiddenNeuronsCount];  

            //Tablice pomocniczne do metody bp z momentem 
            double[][] i2hPrevWeightsDelta = MakeArray(inputNeuronsCount,
                hiddenNeuronsCount); //Poprzedniewagi dla połączeń wartwy wejściowej i ukrytej
            double[] hiddenPrevBiasesDelta = new double[hiddenNeuronsCount]; //Poprzednie biasy warstwy ukrytej
            double[][] h2oPrevWeightsDelta = MakeArray(hiddenNeuronsCount,
                outputNeuronsCount); //Poprzednie wagi dla połączeń wartwy ukrytej i wyjściowej
            double[] outputPrevBiasesDelta = new double[outputNeuronsCount]; //Poprzednie biasy warstwy wyjściowej

            int epoch = 0; //Numer aktualnej epoki uczącej
            double[] xValues = new double[inputNeuronsCount]; // Wartości wejściowe
            double[] targetValues = new double[outputNeuronsCount]; //Wartości oczekiwane
            double derivative = 0.0;
            double errorSignal = 0.0;

            int[] sequence = new int[trainData.Length];
            for (int i = 0; i < sequence.Length; ++i)
            {
                sequence[i] = i;
            }
                
            //Wykonaj maxEpochs epok uczących
            while (epoch < maxEpochs)
            {
                ++epoch;

                Shuffle(sequence); //Funkcja zmienia indeksy wektorów uczących tak aby
                                   //wymieszać ich kolejność
                
                for (int ii = 0; ii < trainData.Length; ++ii) //Dla każdego wektora uczącego
                {
                    int trainDataIndex = sequence[ii]; //Pobierz kolejny indeks wektora uczącego

                    //Skopiuj wartości wejściowe wektora uczącego do tablicy xVlues oraz wartości oczekiwane
                    //do tablicy targetValues
                    Array.Copy(trainData[trainDataIndex], xValues, inputNeuronsCount); 
                    Array.Copy(trainData[trainDataIndex], inputNeuronsCount, targetValues, 0, outputNeuronsCount);

                    //Oblicz wartośi wyjściowe (są kopiowane do tablicy output; wejścia są kopiowane do
                    //tablicy input)
                    GetOutput(xValues); 

                    // indices: i = inputs, j = hiddens, k = outputs

                    //Obliczenie sygnałów wejściowych dla warstwy wyjściowej
                    //(odwrócenie kierunku porpagacji sygnałów)
                    for (int k = 0; k < outputNeuronsCount; ++k)
                    {
                        errorSignal = targetValues[k] - outputs[k]; //Obliczenie błędu
                        derivative = (1 - outputs[k]) * outputs[k]; //Obliczenie pochodnej dla funkcji soft max
                        outputSignals[k] = errorSignal * derivative; //Obliczenie sygnału dla warstwy wyjściowej
                    }

                    //Obliczenie gradientu dla wag połączeń warstwy ukrytej do wyjściowej za pomocą
                    //outputSignals
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                    {
                        for (int k = 0; k < outputNeuronsCount; ++k)
                        {
                            hidden2outputGradients[j][k] = outputSignals[k] * hiddenOutputs[j];
                        }   
                    }
                        
                    //Obliczenie gradientu biasów warstwy wyjściowej
                    //za pomocą outputSignals
                    for (int k = 0; k < outputNeuronsCount; ++k)
                    {
                        outputBiasGradients[k] = outputSignals[k] * 1.0;
                    }
                        
                    //Obliczenie sygnałów dla warstwy ukrytej
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                    {
                        derivative = (1 + hiddenOutputs[j]) * (1 - hiddenOutputs[j]); //Obliczenie pochodnej dla
                                                                                      //tangensa hiperbolicznego
                        double sum = 0.0;
                        for (int k = 0; k < outputNeuronsCount; ++k)
                        {
                            sum += outputSignals[k] * hidden2outputWeights[j][k];
                        }
                        hiddenSignals[j] = derivative * sum;
                    }

                    //Obliczenie gradientu dla wag połączeń warstwy ukrytej do wejściowej
                    for (int i = 0; i < inputNeuronsCount; ++i)
                    {
                        for (int j = 0; j < hiddenNeuronsCount; ++j)
                        {
                            input2hiddenGradients[i][j] = hiddenSignals[j] * input[i];
                        }    
                    }

                    //Obliczenie gradientu biasów warstwy ukrytej
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                    {
                        hiddenBiasGradients[j] = hiddenSignals[j] * 1.0;
                    }   
                    
                    //Aktualizacja wag połączeń między warstwą wejściową a ukrytą
                    for (int i = 0; i < inputNeuronsCount; ++i)
                    {
                        for (int j = 0; j < hiddenNeuronsCount; ++j)
                        {
                            double delta = input2hiddenGradients[i][j] * learnRate;
                            input2hiddenWeights[i][j] += delta;
                            input2hiddenWeights[i][j] += i2hPrevWeightsDelta[i][j] * momentum;
                            i2hPrevWeightsDelta[i][j] = delta;
                        }
                    }

                    //Aktualizacja biasów warstwy ukrytej
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                    {
                        double delta = hiddenBiasGradients[j] * learnRate;
                        hiddenBiases[j] += delta;
                        hiddenBiases[j] += hiddenPrevBiasesDelta[j] * momentum;
                        hiddenPrevBiasesDelta[j] = delta;
                    }

                    //Aktualizacja wag połączeń warstwy ukrytej do wyjściowej
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                    {
                        for (int k = 0; k < outputNeuronsCount; ++k)
                        {
                            double delta = hidden2outputGradients[j][k] * learnRate;
                            hidden2outputWeights[j][k] += delta;
                            hidden2outputWeights[j][k] += h2oPrevWeightsDelta[j][k] * momentum;
                            h2oPrevWeightsDelta[j][k] = delta;
                        }
                    }

                    //Aktualizacja biasów warstwy wyjściowej
                    for (int k = 0; k < outputNeuronsCount; ++k)
                    {
                        double delta = outputBiasGradients[k] * learnRate;
                        outputBiases[k] += delta;
                        outputBiases[k] += outputPrevBiasesDelta[k] * momentum;
                        outputPrevBiasesDelta[k] = delta;
                    }

                }
           
            }
     
        }

        private void Shuffle(int[] sequence) // instance method
        {
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }

    }
}
