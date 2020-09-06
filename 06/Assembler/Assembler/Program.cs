using System;
using System.Collections.Generic;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Amaia\Desktop\FromNand2Tetris\nand2tetris\projects\06\add\Add.asm");
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Amaia\Desktop\FromNand2Tetris\nand2tetris\projects\06\max\Max.asm");
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Amaia\Desktop\FromNand2Tetris\nand2tetris\projects\06\max\MaxL.asm");
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Amaia\Desktop\FromNand2Tetris\nand2tetris\projects\06\pong\Pong.asm");
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Amaia\Desktop\FromNand2Tetris\nand2tetris\projects\06\pong\PongL.asm");
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Amaia\Desktop\FromNand2Tetris\nand2tetris\projects\06\rect\Rect.asm");
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Amaia\Desktop\FromNand2Tetris\nand2tetris\projects\06\rect\RectL.asm");


            //creamos la tabla de LABELS
            Dictionary<string, int> Tabla = new Dictionary<string, int>();
            CrearTabla(Tabla);

            //Normalizamos el programa (quitando espacios y comentarios)
            List<string> lista = new List<string>();
            int index = 0;
            foreach (string line in lines)
            {
                bool important = false;
                string linea = "";
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Equals('/') && line[i + 1].Equals('/'))
                    {
                        //Ignore
                        break;
                    }
                    else if (line[i].Equals(' '))
                    {
                        //Ignore
                    }
                    else
                    {
                        linea += line[i];
                        important = true;
                    }
                }
                if (important)
                {
                    if (linea[0].Equals('('))
                    {
                        Tabla.Add(linea.Substring(1, linea.Length - 2), index);
                    }
                    else
                    {
                        lista.Add(linea);
                        index++;
                    }
                }
            }

            //lista = programa entero sin comentarios
            //cada linea de la lista es una instruction o label

            //LA LOGICA
            int indTabla = 16;
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Amaia\Desktop\FromNand2Tetris\nand2tetris\projects\06\Add.hack", true))
            {
                foreach (string linea in lista)
                {
                    //A-INSTRUCTION
                    if (linea[0].Equals('@'))
                    {
                        string location = linea.Substring(1, linea.Length - 1);
                        string AInstr = "";
                        try
                        {
                            //si la location es un num DECIMAL -> lo convertimos A BINARIO
                            int deci = Convert.ToInt32(location);
                            AInstr = Convert.ToString(deci, 2).PadLeft(16, '0');
                            //PROBLEM: Creo que es aquí donde falla el programa Pong.asm -> hay labels con nombres como "mathf.sqrt(this)" o del estilo
                            //que creo que entran a este try sin problema. Como son labels puestas para romper el programa he usado PongL.asm
                            //directamente. El problema está aquí o al llenar la tabla de labels.
                        }
                        catch (Exception a)
                        {
                            //si es un LABEL, buscarlo en la tabla, si no existe se añade                        
                            if (Tabla.ContainsKey(location)) //si está en la tabla -> get from tabla
                            {
                                AInstr = Convert.ToString(Tabla.GetValueOrDefault(location), 2).PadLeft(16, '0');
                            }
                            else //si no existe ese valor guardado -> add to tabla
                            {
                                while (Tabla.ContainsValue(indTabla))
                                {
                                    indTabla++;
                                }
                                Tabla.Add(location, indTabla);
                                AInstr = Convert.ToString(indTabla, 2).PadLeft(16, '0');
                            }
                        }
                        file.WriteLine(AInstr);
                    }
                    //C-INSTRUCTION
                    else
                    {
                        string dest = "";
                        string comp = "";
                        string jump = "";

                        string CInstr = "111";
                        if (linea.Contains('=') && linea.Contains(';')) //si tengo toda la info
                        {
                            dest = linea.Substring(0, linea.IndexOf('='));
                            string d = DestBinary(dest);
                            comp = linea.Substring(linea.IndexOf('=') + 1, linea.IndexOf(';') - linea.IndexOf('=') + 1);
                            string c = CompBinary(comp);
                            jump = linea.Substring(linea.IndexOf(';') + 1);
                            string j = JumpBinary(jump);

                            CInstr = CInstr + c + d + j;
                        }
                        else if (linea.Contains('=')) //si jump es null
                        {
                            dest = linea.Substring(0, linea.IndexOf('='));
                            string d = DestBinary(dest);
                            comp = linea.Substring(linea.IndexOf('=') + 1);
                            string c = CompBinary(comp);
                            jump = "000";

                            CInstr = CInstr + c + d + jump;
                        }
                        else if (linea.Contains(';')) //si dest es null
                        {
                            dest = "000";
                            comp = linea.Substring(0, linea.IndexOf(';'));
                            string c = CompBinary(comp);
                            jump = linea.Substring(linea.IndexOf(';') + 1);
                            string j = JumpBinary(jump);

                            CInstr = CInstr + c + dest + j;
                        }
                        file.WriteLine(CInstr);
                    }
                }
            }

            //Lleno el Dictionary en el método
            static void CrearTabla(Dictionary<string, int> Tabla)
            {
                for (int i = 0; i < 16; i++)
                {
                    Tabla.Add("R" + i, i);
                }
                Tabla.Add("SCREEN", 16384);
                Tabla.Add("KBD", 24576);
                Tabla.Add("SP", 0);
                Tabla.Add("LCL", 1);
                Tabla.Add("ARG", 2);
                Tabla.Add("THIS", 3);
                Tabla.Add("THAT", 4);
            }

            
            static string DestBinary(string dest)
            {
                switch (dest)
                {
                    case "M":
                        return "001";
                    case "D":
                        return "010";
                    case "MD":
                        return "011";
                    case "A":
                        return "100";
                    case "AM":
                        return "101";
                    case "AD":
                        return "110";
                    default:
                        return "111";
                }
            }

            static string CompBinary(string comp)
            {
                string a = "";
                if (comp.Contains('M'))
                {
                    a = "1";
                    switch (comp)
                    {
                        case "M":
                            return a + "110000";
                        case "!M":
                            return a + "110001";
                        case "-M":
                            return a + "110011";
                        case "M+1":
                            return a + "110111";
                        case "M-1":
                            return a + "110010";
                        case "D+M":
                            return a + "000010";
                        case "D-M":
                            return a + "010011";
                        case "M-D":
                            return a + "000111";
                        case "D&M":
                            return a + "000000";
                        case "D|M":
                            return a + "010101";
                        default:
                            return a;
                    }
                }
                else
                {
                    a = "0";
                    switch (comp)
                    {
                        case "0":
                            return a + "101010";
                        case "1":
                            return a + "111111";
                        case "-1":
                            return a + "111010";
                        case "D":
                            return a + "001100";
                        case "A":
                            return a + "110000";
                        case "!D":
                            return a + "001101";
                        case "!A":
                            return a + "110001";
                        case "-D":
                            return a + "001111";
                        case "-A":
                            return a + "110011";
                        case "D+1":
                            return a + "011111";
                        //case "1+D":
                        //    return a + "011111";
                        case "A+1":
                            return a + "110111";
                        //case "1+A":
                        //    return a + "110111";
                        case "D-1":
                            return a + "001110";
                        case "A-1":
                            return a + "110010";
                        case "D+A":
                            return a + "000010";
                        //case "A+D":
                        //    return a + "000010";
                        case "D-A":
                            return a + "010011";
                        case "A-D":
                            return a + "000111";
                        case "D&A":
                            return a + "000000";
                        case "D|M":
                            return a + "010101";
                        default:
                            return a;
                    }
                }
            }

            static string JumpBinary(string jump)
            {
                switch (jump)
                {
                    case "JGT":
                        return "001";
                    case "JEQ":
                        return "010";
                    case "JGE":
                        return "011";
                    case "JLT":
                        return "100";
                    case "JNE":
                        return "101";
                    case "JLE":
                        return "110";
                    default:
                        return "111";
                }
            }
        }
    }
}
