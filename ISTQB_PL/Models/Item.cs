using ISTQB_PL.ViewModels;
using System;
using System.IO;
using Xamarin.Forms;

namespace ISTQB_PL.Models
{
    public class Item : BaseViewModel
    {
        public string Id { get; set; }
        public string Id_Sorted { get; set; }
        public string MyContent { get; set; }
        public string Answer_a { get; set; }
        public string Answer_b { get; set; }
        public string Answer_c { get; set; }
        public string Answer_d { get; set; }
        public string Answer_right { get; set; }
        public string Answer_user { get; set; }
        public string Str_Picture { get; set; }
        public string Exp_Picture { get; set; }
        public string Str_Explanation { get; set; }
        public string Rozdzial { get; set; }
        public string Wersja_Sylabus {  get; set; }
        public bool Visible_Picture { get; set; }
        public string Id_Exam { get; set; }
        public string Id_Wrong { get; set; }
        public string RadioBtnSelected { get; set; }
        public bool Answer_clicked { get; set; }
        public bool IsSelected { get; set; }
        public string Answer_color { get; set; } //zmienna przechowująca wartość potrzebną do zaznaczenia background frame w collectionview - "tak/nie"
        public int MyFontSize => int.Parse(Application.Current.Properties["FontSize"].ToString());
        public Color ItemMainTextColor { get; set; }
        public bool Answer_checked { get; set; }
        public int Id_ExamNumber { get; set; }
    }
}