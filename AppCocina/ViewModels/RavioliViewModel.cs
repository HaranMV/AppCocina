using AppCocina.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppCocina.ViewModels
{
    public class RavioliViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<RecetaModel> Recetas { get; set; } = new();
        List<RecetaModel> Lista = new();
        public ObservableCollection<string> Estrellas { get; } = ["⭐", "⭐⭐", "⭐⭐⭐", "⭐⭐⭐⭐", "⭐⭐⭐⭐⭐"];
        public RecetaModel Receta { set; get; } = null!;
        public RecetaModel RecetaR { set; get; } = null!;
        public string Error = "";
        public int CaloriasTotales = 0;
        int IndiceEditar;

        Random r = new();

        public ICommand RegresarCommand { get; set; }
        public ICommand IrFavoritosCommand { get; set; }
        public ICommand AddFavoritosCommand { get; set; }
        public ICommand IrListaCommand { get; set; }
        public ICommand IrRecetaCommand { get; set; }
        public ICommand IrRecetaRCommand { get; set; }
        public ICommand BuscarCommand { get; set; }
        public ICommand IrAgregarCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand IrEditarCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }


        public RavioliViewModel()
        {
            RegresarCommand = new Command(Regresar);
            IrFavoritosCommand = new Command(IrFavoritos);
            AddFavoritosCommand = new Command(AddFavoritos);
            IrListaCommand = new Command(IrLista);
            IrRecetaCommand = new Command(IrReceta);
            IrRecetaRCommand = new Command(IrRecetaR);
            BuscarCommand = new Command(Buscar);
            IrAgregarCommand = new Command(IrAgregar);
            AgregarCommand = new Command(Agregar);
            IrEditarCommand = new Command<RecetaModel>(IrEditar);
            EditarCommand = new Command(Editar);
            EliminarCommand = new Command<RecetaModel>(Eliminar);
            Cargar();
            Aleatorio();
            foreach (var item in Lista)
            {
                Recetas.Add(item);
            }
        }



        private void IrAgregar()
        {
            Error = "";
            Receta = new();
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Shell.Current.GoToAsync("//Agregar");

        }

        public void Validar()
        {
            if (String.IsNullOrWhiteSpace(Receta.Nombre))
            {
                Error += "Escriba el nombre de la receta \n";
            }
            if (String.IsNullOrWhiteSpace(Receta.Foto) || !Receta.Foto.StartsWith("https:") || !Receta.Foto.EndsWith(".jpg"))
            {
                Error += "Asigne una foto en formato .jpg a la receta \n";
            }
            if (String.IsNullOrWhiteSpace(Receta.Ingredientes))
            {
                Error += "Asigne ingredientes a la receta \n";
            }
            if (String.IsNullOrWhiteSpace(Receta.Preparacion))
            {
                Error += "Escriba la preparación de la receta \n";
            }
            if (Receta.Tiempo <= 0)
            {
                Error += "Escriba correctamente el tiempo \n";
            }

        }

        public RecetaModel Aleatorio()
        {

            int pos = r.Next(0, Lista.Count());
            RecetaR = Lista[pos];
            PropertyChanged?.Invoke(this, new(nameof(RecetaR)));

            return RecetaR;
        }


        private void Agregar()
        {
            if (Receta != null)
            {
                Validar();

                PropertyChanged?.Invoke(this, new(nameof(Error)));
                if (Error == "")
                {
                    Lista.Add(Receta);
                    Guardar();

                }
            }

            Regresar();
        }
        private void Eliminar(RecetaModel receta)
        {
            if (Receta != null)
            {
                Lista.Remove(Receta);
                Guardar();
                Regresar();
                Shell.Current.DisplayAlert($"Eliminar {Receta.Nombre}", "La receta ha sido eliminada",
                "aceptar");
            }
        }
        private void IrEditar(RecetaModel receta)
        {

            var clon = new RecetaModel
            {
                Nombre = receta.Nombre,
                Favorito = receta.Favorito,
                Calificacion = receta.Calificacion,
                Calorias = receta.Calorias,
                Tiempo = receta.Tiempo,
                Foto = receta.Foto,
                Ingredientes = receta.Ingredientes,
                Preparacion = receta.Preparacion
            };

            Receta = clon;
            Error = "";
            IndiceEditar = Lista.IndexOf(receta);
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Shell.Current.GoToAsync("//Editar");
        }

        private void Editar()
        {
            if (Receta != null)
            {
                Validar();
                if (Error == "")
                {
                    Lista[IndiceEditar] = Receta;
                    Guardar();
                    Shell.Current.GoToAsync("//Receta");
                }
                
            }
        }



        private void Buscar()
        {
            List<RecetaModel> duplicados = new();

            Recetas.Clear();
            var FiltroNombre = Lista.Where(x => x.Nombre.StartsWith(Receta.Nombre) || x.Nombre.Contains(Receta.Nombre));
            var FiltroDuracion = Lista.Where(x => x.Tiempo > Receta.Tiempo);
            var FiltroCalorias = Lista.Where(x => x.Calorias <= Receta.Calorias);

            duplicados.AddRange(FiltroNombre);
            duplicados.AddRange(FiltroDuracion);
            duplicados.AddRange(FiltroCalorias);
            duplicados.Distinct();


            if (duplicados != null)
            {
                foreach (var item in duplicados)
                {
                    Recetas.Add(item);
                }
            }
            Shell.Current.GoToAsync("//ResultadoBusqueda");
        }


        private string corazon = "corazontrue.png";

        public string Corazon
        {
            get { return corazon; }
            set
            {
                corazon = value;
            }
        }

        private string titulo = "Lista de recetas";
       
        public string Titulo
        {
            get { return titulo; }
            set
            {
                titulo = value;
            }
        }

        private void AddFavoritos()
        {
            if (Receta.Favorito == false)
            {
                Receta.Favorito = true;
                corazon = "corazontrue.png";
            }
            else
            {
                Receta.Favorito = false;
                corazon = "corazonfalse.png";
            }
            Guardar();
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            PropertyChanged?.Invoke(this, new(nameof(Corazon)));

        }

        public void IrReceta()
        {

            if (Receta.Favorito == true)
            {
                corazon = "corazontrue.png";
            }
            else
            {
                corazon = "corazonfalse.png";
            }
            PropertyChanged?.Invoke(this, new(nameof(Corazon)));
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Shell.Current.GoToAsync("//Receta");
        }

        private void IrRecetaR()
        {
            Receta = RecetaR;
            IrReceta();
        }


    

        private void IrLista()
        {
            Titulo = "Lista de recetas";
            Recetas.Clear();            
            foreach (var item in Lista)
            {
               Recetas.Add(item);
            }
            PropertyChanged?.Invoke(this, new(nameof(Recetas)));
            PropertyChanged?.Invoke(this, new(nameof(Titulo)));
            Shell.Current.GoToAsync("//ListaReceta");
        }

        private void IrFavoritos()
        {
            Titulo = "Favoritos";

            Recetas.Clear();
            var favoritos = from x in Lista where x.Favorito == true select x;
            foreach (var item in favoritos)
            {
                Recetas.Add(item);
            }
            PropertyChanged?.Invoke(this, new(nameof(Recetas)));
            PropertyChanged?.Invoke(this, new(nameof(Titulo)));
            Shell.Current.GoToAsync("//ListaReceta");

        }


        private void Regresar()
        {
            Error = "";
            PropertyChanged?.Invoke(this, new(nameof(Error)));
            Aleatorio();
            Recetas.Clear();
            foreach (var item in Lista)
            {
                Recetas.Add(item);
            }

         
            Shell.Current.GoToAsync("//Main");
        }

        private void Guardar()
        {
            var ruta = FileSystem.AppDataDirectory;
            File.WriteAllText(ruta + "/recetas.json", JsonSerializer.Serialize(Lista));
        }
        private void Cargar()
        {
            var ruta = FileSystem.AppDataDirectory;
            if (File.Exists(ruta + "/recetas.json"))
            {
                var datos = JsonSerializer.Deserialize<List<RecetaModel>>(File.ReadAllText(ruta + "/recetas.json"));
                if (datos != null)
                {
                    Lista.AddRange(datos);
                }
            }
        }








        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
