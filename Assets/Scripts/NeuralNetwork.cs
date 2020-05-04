
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class NeuralNetwork
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

        public NeuralNetwork(int inputNeuronsCount, int hiddenNeuronsCount, int outputNeuronsCount)
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
        }

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

            Array.Copy(outputSums, outputs, outputSums.Length);
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

        public double[] Train(double[][] trainData, int maxEpochs,
            double learnRate, double momentum)
        {
            // train using back-prop
            // back-prop specific arrays
            double[][] hoGrads = MakeArray(hiddenNeuronsCount, outputNeuronsCount); // hidden-to-output weight gradients
            double[] obGrads = new double[outputNeuronsCount];                   // output bias gradients

            double[][] ihGrads = MakeArray(inputNeuronsCount, hiddenNeuronsCount);  // input-to-hidden weight gradients
            double[] hbGrads = new double[hiddenNeuronsCount];                   // hidden bias gradients

            double[] oSignals = new double[outputNeuronsCount];                  // local gradient output signals - gradients w/o associated input terms
            double[] hSignals = new double[hiddenNeuronsCount];                  // local gradient hidden node signals

            // back-prop momentum specific arrays 
            double[][] ihPrevWeightsDelta = MakeArray(inputNeuronsCount, hiddenNeuronsCount);
            double[] hPrevBiasesDelta = new double[hiddenNeuronsCount];
            double[][] hoPrevWeightsDelta = MakeArray(hiddenNeuronsCount, outputNeuronsCount);
            double[] oPrevBiasesDelta = new double[outputNeuronsCount];

            int epoch = 0;
            double[] xValues = new double[inputNeuronsCount]; // inputs
            double[] tValues = new double[outputNeuronsCount]; // target values
            double derivative = 0.0;
            double errorSignal = 0.0;

            int[] sequence = new int[trainData.Length];
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            int errInterval = maxEpochs / 10; // interval to check error
            while (epoch < maxEpochs)
            {
                ++epoch;

                

                Shuffle(sequence); // visit each training data in random order
                for (int ii = 0; ii < trainData.Length; ++ii)
                {
                    int idx = sequence[ii];
                    Array.Copy(trainData[idx], xValues, inputNeuronsCount);
                    Array.Copy(trainData[idx], inputNeuronsCount, tValues, 0, outputNeuronsCount);
                    GetOutput(xValues); // copy xValues in, compute outputs 

                    // indices: i = inputs, j = hiddens, k = outputs

                    // 1. compute output node signals (assumes softmax)
                    for (int k = 0; k < outputNeuronsCount; ++k)
                    {
                        errorSignal = tValues[k] - outputs[k];  // Wikipedia uses (o-t)
                        derivative = (1 - outputs[k]) * outputs[k]; // for softmax
                        oSignals[k] = errorSignal * derivative;
                    }

                    // 2. compute hidden-to-output weight gradients using output signals
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                        for (int k = 0; k < outputNeuronsCount; ++k)
                            hoGrads[j][k] = oSignals[k] * hiddenOutputs[j];

                    // 2b. compute output bias gradients using output signals
                    for (int k = 0; k < outputNeuronsCount; ++k)
                        obGrads[k] = oSignals[k] * 1.0; // dummy assoc. input value

                    // 3. compute hidden node signals
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                    {
                        derivative = (1 + hiddenOutputs[j]) * (1 - hiddenOutputs[j]); // for tanh
                        double sum = 0.0; // need sums of output signals times hidden-to-output weights
                        for (int k = 0; k < outputNeuronsCount; ++k)
                        {
                            sum += oSignals[k] * hidden2outputWeights[j][k]; // represents error signal
                        }
                        hSignals[j] = derivative * sum;
                    }

                    // 4. compute input-hidden weight gradients
                    for (int i = 0; i < inputNeuronsCount; ++i)
                        for (int j = 0; j < hiddenNeuronsCount; ++j)
                            ihGrads[i][j] = hSignals[j] * input[i];

                    // 4b. compute hidden node bias gradients
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                        hbGrads[j] = hSignals[j] * 1.0; // dummy 1.0 input

                    // == update weights and biases

                    // update input-to-hidden weights
                    for (int i = 0; i < inputNeuronsCount; ++i)
                    {
                        for (int j = 0; j < hiddenNeuronsCount; ++j)
                        {
                            double delta = ihGrads[i][j] * learnRate;
                            input2hiddenWeights[i][j] += delta; // would be -= if (o-t)
                            input2hiddenWeights[i][j] += ihPrevWeightsDelta[i][j] * momentum;
                            ihPrevWeightsDelta[i][j] = delta; // save for next time
                        }
                    }

                    // update hidden biases
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                    {
                        double delta = hbGrads[j] * learnRate;
                        hiddenBiases[j] += delta;
                        hiddenBiases[j] += hPrevBiasesDelta[j] * momentum;
                        hPrevBiasesDelta[j] = delta;
                    }

                    // update hidden-to-output weights
                    for (int j = 0; j < hiddenNeuronsCount; ++j)
                    {
                        for (int k = 0; k < outputNeuronsCount; ++k)
                        {
                            double delta = hoGrads[j][k] * learnRate;
                            hidden2outputWeights[j][k] += delta;
                            hidden2outputWeights[j][k] += hoPrevWeightsDelta[j][k] * momentum;
                            hoPrevWeightsDelta[j][k] = delta;
                        }
                    }

                    // update output node biases
                    for (int k = 0; k < outputNeuronsCount; ++k)
                    {
                        double delta = obGrads[k] * learnRate;
                        outputBiases[k] += delta;
                        outputBiases[k] += oPrevBiasesDelta[k] * momentum;
                        oPrevBiasesDelta[k] = delta;
                    }

                } // each training item

            } // while
            double[] bestWts = GetWeights();
            return bestWts;
        } 

        private void Shuffle(int[] sequence)
        {
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = this.rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }
    }
}
