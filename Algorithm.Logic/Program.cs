using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Algorithm.Logic
{
    public class Program
    {
        public static int x { get; set; }
        public static int y { get; set; }

        public static List<char> numericsList { get; set; } = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };


        /// <summary>
        /// PROBLEMA:
        /// 
        /// Implementar um algoritmo para o controle de posição de um drone emum plano cartesiano (X, Y).
        /// 
        /// O ponto inicial do drone é "(0, 0)" para cada execução do método Evaluate ao ser executado cada teste unitário.
        /// 
        /// A string de entrada pode conter os seguintes caracteres N, S, L, e O representando Norte, Sul, Leste e Oeste respectivamente.
        /// Estes catacteres podem estar presentes aleatóriamente na string de entrada.
        /// Uma string de entrada "NNNLLL" irá resultar em uma posição final "(3, 3)", assim como uma string "NLNLNL" irá resultar em "(3, 3)".
        /// 
        /// Caso o caracter X esteja presente, o mesmo irá cancelar a operação anterior. 
        /// Caso houver mais de um caracter X consecutivo, o mesmo cancelará mais de uma ação na quantidade em que o X estiver presente.
        /// Uma string de entrada "NNNXLLLXX" irá resultar em uma posição final "(1, 2)" pois a string poderia ser simplificada para "NNL".
        /// 
        /// Além disso, um número pode estar presente após o caracter da operação, representando o "passo" que a operação deve acumular.
        /// Este número deve estar compreendido entre 1 e 2147483647.
        /// Deve-se observar que a operação 'X' não suporta opção de "passo" e deve ser considerado inválido. Uma string de entrada "NNX2" deve ser considerada inválida.
        /// Uma string de entrada "N123LSX" irá resultar em uma posição final "(1, 123)" pois a string pode ser simplificada para "N123L" (CORREÇÃO, UMA ENTRADA N123LSX RESULTA EM (123,1))
        /// Uma string de entrada "NLS3X" irá resultar em uma posição final "(1, 1)" pois a string pode ser siplificada para "NL".
        /// 
        /// Caso a string de entrada seja inválida ou tenha algum outro problema, o resultado deve ser "(999, 999)".
        /// 
        /// OBSERVAÇÕES:
        /// Realizar uma implementação com padrões de código para ambiente de "produção". 
        /// Comentar o código explicando o que for relevânte para a solução do problema.
        /// Adicionar testes unitários para alcançar uma cobertura de testes relevânte.
        /// </summary>
        /// <param name="input">String no padrão "N1N2S3S4L5L6O7O8X"</param>
        /// <returns>String representando o ponto cartesiano após a execução dos comandos (X, Y)</returns>
        public static string Evaluate(string input)
        {
            x = 0;
            y = 0;

            if (InputValidation(input) != true)
                return "(999, 999)";

            string simplifiedInput = SimplifyInput(input);

            for (int i = 0; i < simplifiedInput.Length; i++)
            {
                char direction = simplifiedInput[i];
                int steps = 1;

                //Verifica se o caracter atual não é númerico e se existe um próximo caracter
                if (i + 1 < simplifiedInput.Length && !numericsList.Contains(simplifiedInput[i]))
                {
                    //Verifica se o proximo caracter não é númerico
                    if (!numericsList.Contains(simplifiedInput[i + 1]))
                    {
                        Move(direction, steps);
                    }
                    else
                    {
                        int range = 0;
                        //Procura pelo próximo caracter não númerico
                        for (int j = i + 1; j < simplifiedInput.Length; j++, range++)
                        {
                            if (!numericsList.Contains(simplifiedInput[j]))
                            {
                                break;
                            }
                        }

                        //Faz a separação do range entrado entre o caracter atual e o próximo não númerico
                        var inputNumber = simplifiedInput.Skip(i + 1).Take(range).ToArray();

                        steps = int.Parse(new string(inputNumber));
                        Move(direction, steps);
                        i = i + inputNumber.Length;
                    }
                }
                else
                {
                    Move(direction, steps);
                }

            }

            return $"({x}, {y})";
        }

        /// <summary>
        /// Move os pontos nos eixos X e Y no plano cartesiano de acordo com a direção (N,S,L,O) e o valor de passos a se dar na direção desejada
        /// </summary>
        /// <param name="direction">Direção desejada (N,S,L,O)</param>
        /// <param name="steps">Quantidade de passos a serem dados em determinada direção</param>
        private static void Move(char direction, int steps = 1)
        {
            switch (direction)
            {
                case 'N':
                    x = x + steps;
                    break;
                case 'S':
                    x = x - steps;
                    break;
                case 'O':
                    y = y - steps;
                    break;
                case 'L':
                    y = y + steps;
                    break;
            }
        }

        /// <summary>
        /// Função de simplicação do input recebido, trabalhado sobre o conceito de cancelamento de operação anterior
        /// </summary>
        /// <param name="input">String de entrada bruta (N123LSX)</param>
        /// <returns>String simplificada (N123L)</returns>
        private static string SimplifyInput(string input)
        {
            while (input.Contains("X"))
            {
                input = RemoveX(input);
            }
            return input;
        }

        /// <summary>
        /// Tratamento para simplificação da string na ocorrencia do primeiro X.
        /// </summary>
        /// <param name="input">String a ser simplificada (NNNXLLLXX)</param>
        /// <returns>NNLLLXX</returns>
        private static string RemoveX(string input)
        {
            int index = input.IndexOf('X');

            if (input[index - 1] != 'X' && !numericsList.Contains(input[index - 1]))
            {
                char[] inputArray = input.ToCharArray();
                inputArray[index] = 'A';
                inputArray[index - 1] = 'A';
                input = new string(inputArray);
            }
            else if (numericsList.Contains(input[index - 1]))
            {
                string inputSubStringWithNumeric = input.Substring(0, index);

                int firstCharPos = FindFristCharPosition(inputSubStringWithNumeric);

                inputSubStringWithNumeric = inputSubStringWithNumeric.Substring(firstCharPos);

                string formatedSubStringWithoutNumeric = new string(inputSubStringWithNumeric.Select(c => numericsList.Contains(c) ? 'A' : c).ToArray());

                input = ReplaceLastOccurrence(input, inputSubStringWithNumeric, formatedSubStringWithoutNumeric);

            }

            return input.Replace("A", "");
        }

        /// <summary>
        /// Retorna a posição do primeiro caracter não númerico
        /// </summary>
        /// <param name="input">123NL52</param>
        /// <returns>2</returns>
        public static int FindFristCharPosition(string input)
        {
            int pos = 0;
            for (int i = input.Length - 1; i != 0; i--)
            {
                if (!numericsList.Contains(input[i]) && pos == 0)
                {
                    pos = i;
                    break;
                }
            }
            return pos;
        }

        /// <summary>
        /// Troca a ultima ocorrencia de uma entrada por outra
        /// </summary>
        /// <param name="input">String a de entrada</param>
        /// <param name="find">String a ser procurada</param>
        /// <param name="replace">String de substituição</param>
        /// <returns></returns>
        public static string ReplaceLastOccurrence(string input, string find, string replace)
        {
            int pos = input.LastIndexOf(find);

            if (pos == -1)
                return input;

            string result = input.Remove(pos, find.Length).Insert(pos, replace);
            return result;
        }

        /// <summary>
        /// Faz uso de uma Regex e outras regras para validar o formato de entrada do algoritmo
        /// </summary>
        /// <param name="input">String de entrada</param>
        /// <returns>Verdadeiro para entrada válida e falso para entrada inválida</returns>
        private static bool InputValidation(string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return false;

            string regex = "^[NSLO]+[NSLOX0-9]*(?<!X+[0-9])";
            Match match = Regex.Match(input, regex);
            if (!match.Success || !match.Value.ToString().Equals(input))
            {
                return false;
            }

            if (input.Contains("2147483647"))
            {
                int post = input.IndexOf("2147483647");

                if (input[post + 10] != 'X')
                    return false;
            }

            return true;
        }
    }
}
