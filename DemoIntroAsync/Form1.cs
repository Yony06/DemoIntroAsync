using System.Diagnostics;

namespace DemoIntroAsync
{
    public partial class Form1 : Form
    {

        HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible=true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoParalelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecution(destinoParalelo,destinoBaseSecuencial);

            Console.WriteLine("Inico");
            List<Imagen> imagenes = ObtenerImagenes();

            var sw = new Stopwatch();
            sw.Start();

          foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }
            Console.WriteLine("Secuencia- duracion en segundos; {0}",
                sw.ElapsedMilliseconds / 1000,0);

            sw.Reset();
            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoParalelo, imagen);
            });
                

            await Task.WhenAll(tareasEnumerable);
            Console.WriteLine("Paralelo- duracion en segundos; {0}",
               sw.ElapsedMilliseconds / 1000, 0);

            sw.Stop();


            pictureBox1.Visible = false;
        }

        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using( var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }

            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);


        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Castillo de Sancti Petri  {i}.png",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/f/f2/Castillo_de_Sancti_Petri.JPG"
                    });
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Bisonte Magdaleniense polícromo {i}.png",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/8/8b/9_Bisonte_Magdaleniense_pol%C3%ADcromo.jpg"
                    });
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Dama de Elche  {i}.png",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/1/18/Dama_de_Elche.jpg"
                    });

            }

            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach(var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecution(string destinoBaseParalelo, string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {

                Directory.CreateDirectory(destinoBaseSecuencial);
            }
            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBaseParalelo); 
        }

        private async Task<String> ProcesamientoLargo()
        {
            await Task.Delay(3000);
            return " Felipe";
            //minuto 23
        }

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso A finalizado");
        }
        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso B finalizado");
        }
        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso C finalizado");
        }
    }
}
