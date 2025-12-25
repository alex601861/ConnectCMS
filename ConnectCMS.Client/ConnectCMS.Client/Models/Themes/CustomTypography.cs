using MudBlazor;

namespace CMSTrain.Client.Models.Themes;

public static class CustomTypography
{
    public static Typography CmsTypography(string fontFamily = Constants.Constants.FontFamily.Poppins)
    {
        return new Typography
        {
            Default = new Default
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(14px, calc(0.875rem + ((1vw - 7.68px) * 0.1736)), 16px)",
                FontWeight = 400,
                LineHeight = 1.43,
            },
            H1 = new H1
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(20px, calc(1.25rem + ((1vw - 3.2px) * 0.625)), 30px)",
                FontWeight = 700,
                LineHeight = 1.167,
            },
            H2 = new H2
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(18px, calc(1.125rem + ((1vw - 3.2px) * 0.5)), 26px)",
                FontWeight = 700,
                LineHeight = 1.2,
            },
            H3 = new H3
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(18px, calc(1.125rem + ((1vw - 3.2px) * 0.375)), 24px)",
                FontWeight = 600,
                LineHeight = 1.167,
            },
            H4 = new H4
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(16px, calc(1rem + ((1vw - 3.2px) * 0.375)), 22px)",
                FontWeight = 600,
                LineHeight = 1.235,
            },
            H5 = new H5
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(15px, calc(0.9375rem + ((1vw - 3.2px) * 0.25)), 19px)",
                FontWeight = 600,
                LineHeight = 1.334,
            },
            H6 = new H6
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(14px, calc(0.875rem + ((1vw - 3.2px) * 0.1875)), 17px)",
                FontWeight = 500,
                LineHeight = 1.6,
            },
            //Button = new Button
            //{
            //    FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
            //    FontSize = "13px",
            //    FontWeight = 400,
            //    LineHeight = 1.75,
            //},
            Body1 = new Body1
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(13px, calc(0.8125rem + ((1vw - 3.2px) * 0.125)), 15px)",
                FontWeight = 400,
                LineHeight = 1.5,
            },
            Body2 = new Body2
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "13px",
                FontWeight = 400,
                LineHeight = 1.43,
            },
            Caption = new Caption
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "12px",
                FontWeight = 400,
                LineHeight = 1.66,
            },
            Subtitle1 = new Subtitle1
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "clamp(13px, calc(0.8125rem + ((1vw - 3.2px) * 0.125)), 15px)",
                FontWeight = 500,
                LineHeight = 1.57,
            },
            Subtitle2 = new Subtitle2
            {
                FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
                FontSize = "13px",
                FontWeight = 500,
                LineHeight = 1.57,
            }
        };
    }
    
    // TODO: Configure dynamic themes for typography on non-production server.
    // public static Typography CmsTypography(string fontFamily = Constants.Constants.FontFamily.Poppins)
    // {
    //     return new Typography
    //     {
    //         Default = new Default
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(16px, calc(0.875rem + ((1vw - 7.68px) * 0.1736)), 18px)",
    //             FontWeight = 400,
    //             LineHeight = 1.43,
    //         },
    //         H1 = new H1
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(22px, calc(1.25rem + ((1vw - 3.2px) * 0.625)), 32px)",
    //             FontWeight = 700,
    //             LineHeight = 1.167,
    //         },
    //         H2 = new H2
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(20px, calc(1.125rem + ((1vw - 3.2px) * 0.5)), 28px)",
    //             FontWeight = 700,
    //             LineHeight = 1.2,
    //         },
    //         H3 = new H3
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(20px, calc(1.125rem + ((1vw - 3.2px) * 0.375)), 26px)",
    //             FontWeight = 600,
    //             LineHeight = 1.167,
    //         },
    //         H4 = new H4
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(18px, calc(1rem + ((1vw - 3.2px) * 0.375)), 24px)",
    //             FontWeight = 600,
    //             LineHeight = 1.235,
    //         },
    //         H5 = new H5
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(16px, calc(0.9375rem + ((1vw - 3.2px) * 0.25)), 20px)",
    //             FontWeight = 600,
    //             LineHeight = 1.334,
    //         },
    //         H6 = new H6
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(15px, calc(0.875rem + ((1vw - 3.2px) * 0.1875)), 18px)",
    //             FontWeight = 500,
    //             LineHeight = 1.6,
    //         },
    //         Body1 = new Body1
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(14px, calc(0.8125rem + ((1vw - 3.2px) * 0.125)), 16px)",
    //             FontWeight = 400,
    //             LineHeight = 1.5,
    //         },
    //         Body2 = new Body2
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "14px",
    //             FontWeight = 400,
    //             LineHeight = 1.43,
    //         },
    //         Caption = new Caption
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "12px",
    //             FontWeight = 400,
    //             LineHeight = 1.66,
    //         },
    //         Subtitle1 = new Subtitle1
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "clamp(14px, calc(0.8125rem + ((1vw - 3.2px) * 0.125)), 16px)",
    //             FontWeight = 500,
    //             LineHeight = 1.57,
    //         },
    //         Subtitle2 = new Subtitle2
    //         {
    //             FontFamily = [fontFamily, "Helvetica", "Arial", "sans-serif"],
    //             FontSize = "14px",
    //             FontWeight = 500,
    //             LineHeight = 1.57,
    //         }
    //     };
    // }
}