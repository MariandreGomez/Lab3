using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab3
{
    public class CSVData //clase para leer el archivo csv
    {
        public CSVData() { }

        public CSVData(string v1, string v2)
        {
            operacion = v1;
            JSONData = v2;
        }

        public string operacion { get; set; }
        public string JSONData { get; set; }

    }

    public class Persona
    {
        public Persona(string nameC, string dpiC, string datebirthC, string adrressC)
        {
            name = nameC;
            dpi = dpiC;
            datebirth = datebirthC;
            address = adrressC;
        }
        public Persona() { }
        public string name { get; set; }
        public string dpi { get; set; }
        public string datebirth { get; set; }
        public string address { get; set; }
        public List<string> companies { get; set; }
    }

    internal class Operaciones
    {
        TreeNode insertarObjeto = new TreeNode(); //arbol 
        public void CSV()
        {
            string[] CsvLines = System.IO.File.ReadAllLines(@"C:\Users\maria\Desktop\inputs (1)\input (2).csv");

            //Listas auxiliares
            List<CSVData> insert = new List<CSVData>();
            List<CSVData> patch = new List<CSVData>();
            List<CSVData> eliminar = new List<CSVData>();

            for (int i = 0; i < CsvLines.Length; i++)
            {
                string[] rowdata = CsvLines[i].Split(';'); // lee el separador ";" 
                CSVData record = new CSVData(rowdata[0], rowdata[1]); //se inserta en la clase que contiene el jsondata y la operacion

                if (rowdata[0] == "INSERT")
                {
                    insert.Add(record);
                }
                else if (rowdata[0] == "PATCH")
                {
                    patch.Add(record);
                }
                else if (rowdata[0] == "DELETE")
                {
                    eliminar.Add(record);
                }
            }

            //Insertar en arbol 
            int num = 0;
            foreach (CSVData item in insert)
            {
                Persona person = JsonConvert.DeserializeObject<Persona>(item.JSONData);
                insertarObjeto.Insertar(person);
            }

            //Actualizar informacion 
            string dpi = "";
            foreach (var item in patch)
            {
                Persona personaPatch = JsonConvert.DeserializeObject<Persona>(item.JSONData);
                dpi = personaPatch.dpi;
                insertarObjeto.ActualizarNodo(dpi, personaPatch);
            }

            //Eliminar informacion          
            foreach (var item in eliminar)
            {
                Persona personaDelete = JsonConvert.DeserializeObject<Persona>(item.JSONData);
                insertarObjeto.Eliminar(personaDelete);
            }

            // num = insertarObjeto.ContarElementos();
            //insertarObjeto.MostrarArbol();
            //Console.WriteLine(Convert.ToString(num));
        }

        public  string clave = "SECRET"; //Clave para transpocision simple 
        public void EncriptarArchivos()
        {
            string carpeta = @"C:\Users\maria\Desktop\inputs (1)\inputs";
            string Cifrado = @"C:\Users\maria\Desktop\inputs (1)\Cifrado";
            string nombre, dpi;
            
            if (Directory.Exists(carpeta))
            {
                string[] archivos = Directory.GetFiles(carpeta, "REC-*.txt");

                foreach (var item in archivos)
                {

                    // Obtener el contenido del archivo
                    string contenido = File.ReadAllText(item);
                    //obtener nombre completo del txt
                    nombre = Path.GetFileNameWithoutExtension(item);
                    //separamos el nombre del archivo para obtener el dpi
                    string[] partesNombre = nombre.Split('-');
                    dpi = partesNombre[1];

                    //Cifrar contenido
                    string contenidoCifrado =Cifrar(contenido,clave);
                    // Crear una nueva ruta para el archivo cifrado en la carpeta de salida
                    string rutaArchivoCifrado = Path.Combine(Cifrado, $"{nombre}-Cifrado.txt");
                    ///Guardar nuevo archivo en una nueva carpeta
                    File.WriteAllText(rutaArchivoCifrado, contenidoCifrado);

                                    
                }
            }           
        }

        public void Decifrar()
        {
            string ArchivoCifrado = @"C:\Users\maria\Desktop\inputs (1)\Cifrado";
            string nombre, dpi,DPIBuscar;

            Console.WriteLine("Ingrese DPI de la persona que desea buscar");
            DPIBuscar = Console.ReadLine();

            Persona personaEncontrada = insertarObjeto.BuscarPorDPI(DPIBuscar);
            if (personaEncontrada != null)
            {
                Console.WriteLine("Persona encontrada(Nombre): " + personaEncontrada.name);
                Console.WriteLine("Persona encontrada(DPI): " + personaEncontrada.dpi);
                Console.WriteLine("Persona encontrada(Direccion): " + personaEncontrada.address);
                Console.WriteLine( );

                if (Directory.Exists(ArchivoCifrado))
                {
                    string[] archivosCifrados = Directory.GetFiles(ArchivoCifrado, "REC-*.txt");
                    foreach (var item in archivosCifrados)
                    {
                        // Obtener nombre completo del archivo cifrado
                        nombre = Path.GetFileNameWithoutExtension(item);
                        // Separar el nombre del archivo para obtener el DPI
                        string[] partesNombre = nombre.Split('-');
                        dpi = partesNombre[1];

                        if (dpi == DPIBuscar)
                        {
                            // Obtener el contenido cifrado del archivo
                            string contenidoCifrado = File.ReadAllText(item);
                            // Descifrar el contenido
                            string contenidoDescifrado = Descifrar(contenidoCifrado,clave);
                            // Mostrar el contenido descifrado
                            Console.WriteLine($"Contenido descifrado de {nombre}:");
                            Console.WriteLine();
                            Console.WriteLine(contenidoDescifrado);
                            Console.WriteLine("-------------------------------------");
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("Persona no encontrada.");
            }
        }

        public string Cifrar(string mensaje, string clave)
        {
            int numRows = (int)Math.Ceiling((double)mensaje.Length / clave.Length);
            char[,] grid = new char[numRows, clave.Length];

            int messageIndex = 0;

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < clave.Length; j++)
                {
                    if (messageIndex < mensaje.Length)
                    {
                        grid[i, j] = mensaje[messageIndex];
                        messageIndex++;
                    }
                    else
                    {
                        grid[i, j] = ' ';
                    }
                }
            }

            int[] order = new int[clave.Length];
            for (int i = 0; i < clave.Length; i++)
            {
                order[i] = i;
            }

            for (int i = 0; i < clave.Length - 1; i++)
            {
                for (int j = i + 1; j < clave.Length; j++)
                {
                    if (clave[i] > clave[j])
                    {
                        char temp = clave[i];
                        clave = clave.Remove(i, 1).Insert(i, clave[j].ToString());
                        clave = clave.Remove(j, 1).Insert(j, temp.ToString());

                        int tempOrder = order[i];
                        order[i] = order[j];
                        order[j] = tempOrder;
                    }
                }
            }

            string mensajeCifrado = "";

            for (int i = 0; i < clave.Length; i++)
            {
                int col = Array.IndexOf(order, i);

                for (int j = 0; j < numRows; j++)
                {
                    mensajeCifrado += grid[j, col];
                }
            }

            return mensajeCifrado;
        }

        public string Descifrar(string mensajeCifrado, string clave)
        {
            int numRows = (int)Math.Ceiling((double)mensajeCifrado.Length / clave.Length);
            char[,] grid = new char[numRows, clave.Length];

            int[] order = new int[clave.Length];
            for (int i = 0; i < clave.Length; i++)
            {
                order[i] = i;
            }

            for (int i = 0; i < clave.Length - 1; i++)
            {
                for (int j = i + 1; j < clave.Length; j++)
                {
                    if (clave[i] > clave[j])
                    {
                        char temp = clave[i];
                        clave = clave.Remove(i, 1).Insert(i, clave[j].ToString());
                        clave = clave.Remove(j, 1).Insert(j, temp.ToString());

                        int tempOrder = order[i];
                        order[i] = order[j];
                        order[j] = tempOrder;
                    }
                }
            }

            int colLength = mensajeCifrado.Length / numRows;
            int index = 0;

            for (int i = 0; i < clave.Length; i++)
            {
                int col = Array.IndexOf(order, i);

                for (int j = 0; j < numRows; j++)
                {
                    grid[j, col] = mensajeCifrado[index];
                    index++;
                }
            }

            string mensajeDescifrado = "";

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < clave.Length; j++)
                {
                    mensajeDescifrado += grid[i, j];
                }
            }

            return mensajeDescifrado.Trim();
        }

    }
}
