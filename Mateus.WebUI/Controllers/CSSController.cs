using System.Web.Mvc;
using Mateus.Model.EFModel;

namespace Mateus.Controllers
{
    public class CSSController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public CSSController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        public ContentResult Generate()
        {
            string css = "";

            int startResolution = 800;
            int endResolution = 1920;
            int resolutionIncrement = 160;
            int numberOfGrids = 10;

            int formMargin = 10;
            int formPadding = 15;
            int formBorder = 1;

            int fieldsetPadding = 30;
            int fieldsetBorder = 1;

            int inputFieldWidthStart = 5;
            int inputFieldWidthEnd = 100;
            int inputFieldIncrement = 5;

            double labelWidthPercentages = 0.35;

            for(; startResolution < endResolution; startResolution+= resolutionIncrement)
            {
                int minWidth = startResolution;
                int maxWidth = startResolution + resolutionIncrement;

                css+= "@media (min-width: " + minWidth + "px) and (max-width: " + maxWidth + "px) {\n\n";

                if(startResolution < 400)
                {
                    css+= ".grid-1 { width: 100% }";
                    css+= ".grid-2 { width: 100%; }";
                    css+= ".grid-3 { width: 100%; }";
                    css+= ".grid-4 { width: 100%; }";
                    css+= ".grid-5 { width: 100%; }";
                    css+= ".grid-6 { width: 100%; }";
                    css+= ".grid-7 { width: 100%; }";
                    css+= ".grid-8 { width: 100%; }";
                    css+= ".grid-9 { width: 100%; }";
                    css+= ".grid-10 { width: 100%; }";
                }

                for(int i = 1; i <= numberOfGrids; i++)
                {
                    // grid width is equal to resolution/numberOfGrids * grid number
                    int gridWidth = (startResolution/numberOfGrids)*i;

                    int inputFieldOnePercent = gridWidth/inputFieldWidthEnd;

                    for(int j = inputFieldWidthStart; j <= inputFieldWidthEnd; j+= inputFieldIncrement)
                    {
                        int wInputWidth = inputFieldOnePercent*j;
                        int wSelectWidth = inputFieldOnePercent*j + 16;

                        css+= "\t.grid-" + i + " input.w" + j + " { width: " + wInputWidth + "px }\n";
                        css+= "\t.grid-" + i + " select.w" + j + " { width: " + wSelectWidth + "px }\n";
                    }

                    // row width is equal to grid width - everything that surrounds this row
                    int rowWidth = gridWidth - (formMargin*2) - (formPadding*2) - (formBorder*2) - (fieldsetPadding*2) - (fieldsetBorder*2) - 10;
                    int labelWidth = (int)(rowWidth * labelWidthPercentages);

                    // input is dependent on label
                    int inputLeftMargin = labelWidth + 10;
                    int inputWidth = rowWidth - inputLeftMargin;

                    if(labelWidth < 100) 
                    {
                        css+= "\t.grid-" + i + " .form .row { width: " + rowWidth + "px; }\n";
                        css+= "\t.grid-" + i + " .form .row label { display: block; clear: both; margin-bottom: 8px; position: relative; width: " + rowWidth + "px; }\n";
                        inputLeftMargin = 0;
                        inputWidth = rowWidth;
                    }
                    else
                    {
                        css+= "\t.grid-" + i + " .form .row { width: " + rowWidth + "px }\n";
                        css+= "\t.grid-" + i + " .form .row label { width: " + labelWidth + "px }\n";
                    }
                    css+= "\t.grid-" + i + " .form .row .text,\n\t.grid-" + i + " .form .row .input { width: " + inputWidth + "px; margin-left: " + inputLeftMargin + "px; padding-left: 0; }\n";
                    css+= "\t.grid-" + i + " .form div.error { margin-left: " + inputLeftMargin + "px; }\n";

                    inputFieldOnePercent = inputWidth/inputFieldWidthEnd;

                    for(int j = inputFieldWidthStart; j <= inputFieldWidthEnd; j+= inputFieldIncrement)
                    {
                        int wInputWidth = inputFieldOnePercent*j;
                        int wInputPaddingWidth = wInputWidth-29;
                        int wInputPaddingWidthFocus = wInputWidth-31;
                        int wSelectWidth = inputFieldOnePercent*j + 16;

                        css+= "\t.grid-" + i + " .form .row input.w" + j + " { width: " + wInputWidth + "px }\n";
                        css+= "\t.grid-" + i + " .form .row input.w" + j + ".autocomplete,\n";
                        css+= "\t.grid-" + i + " .form .row input.w" + j + ".autocomplete-with-hidden-field { width: " + wInputPaddingWidth + "px }\n";
                        css+= "\t.grid-" + i + " .form .row select.w" + j + " { width: " + wSelectWidth + "px }\n";
                    }

                    css+= "\n";
                }

                css+= "}\n\n";
            }

            return Content(css, "text/css");
        }
    }
}