using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace PowerLyrics.Core
{
    public enum LyricType
    {
        Verse,
        Chorus,
        Bridge,
        Undefined
    }

    public enum SlideType
    {
        Slide,
        Divider
    }

    public enum FileType
    {
        Song,
        PlayList,
        undefined
    }

    class constants
    {
        public const int FONT_SIZE = 25;
        //public static string DEFAULT_TEXT = "Mládežka " + DateTime.Now.ToString("dd.MM.yyyy");
        public static string DEFAULT_TEXT = "Ahojte!\n" + DateTime.Now.ToString("dd.MM.yyyy");
        public static FontFamily DEFAULT_FONT_FAMILY = new FontFamily("Segoe UI");
        public static TextAlignment DEFAULT_TEXT_ALIGNMENT = TextAlignment.Center;

        public static int MAGICNUMBER_FILE = 0x50574c59; // PWLY
        public static int MAGICNUMBER_SONG = 0x534f4e47; // SONG
        public static int MAGICNUMBER_PLAYLIST = 0x504c4c49; // PLLI

    }

    // https://www.youtube.com/watch?v=Bp5LFXjwtQ0 
    public class EnumBindingSourceExtencion : MarkupExtension
    {
        public Type EnumType { get; set; }

        public EnumBindingSourceExtencion(Type enumType)
        {
            EnumType = enumType;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
