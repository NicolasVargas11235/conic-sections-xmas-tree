using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

/**
Program Name: Conic Sections
Author: Nicolas Vargas
Date completed: 1/4/2022

Special Thanks to Jose Luis from Parametric Camp for providing public
lectures/tutorials on modeling a christmas tree using Rhino and Grasshopper.

Link the Parametric Camp youtube channel: https://www.youtube.com/channel/UCSgG9KzVsS6jArapCx-Bslg
**/

//Due to Grasshopper restrictions, this class is used
//to convert the following variables to global variables
static class Globals
{
  public static List<Point3d> leds;
  public static double coefA;
  public static double coefB;
  public static double coefC;
  public static double fullTravel;
  public static double frameCount;
  public static double step;
  public static double ledCount;
  public static double spinPoint; //at which conic section the tree is rotated
  public static List<string> csv;
}

/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public class Script_Instance : GH_ScriptInstance
{
#region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { __out.Add(text); }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { __out.Add(string.Format(format, args)); }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj)); }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj, method_name)); }
#endregion

#region Members
  /// <summary>Gets the current Rhino document.</summary>
  private RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private IGH_Component Component; 
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private int Iteration;
#endregion

  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments, 
  /// Output parameters as ref arguments. You don't have to assign output parameters, 
  /// they will have a default value.
  /// </summary>
  private void RunScript(List<Point3d> leds, int fps, int duration, ref object CSV)
  {
        // Count the number of LEDs on the tree
    Globals.ledCount = leds.Count;

    // Prepare the first row of the CSV file
    Globals.leds = leds;
    Globals.csv = new List<string>();

    // Add the header, which looks like:
    // "FRAME_ID,R_0,G_0,B_0,R_1,G_1,B_1..."
    string row = "FRAME_ID,";
    for (int i = 0; i < Globals.ledCount; i++)
    {
      row += "R_" + i + ",G_" + i + ",B_" + i;
      if (i != Globals.ledCount - 1)
      {
        row += ",";
      }
    }
    Globals.csv.Add(row);

    // Total length of animation (# of frames) for one conic section
    Globals.frameCount = fps * duration;

    //Animate all conic sections
    animateCircle();
    animateEllipse();
    animateParabola();
    animateHyperbola();

    // Outputs
    CSV = Globals.csv;

  }

  // <Custom additional code> 
  
  //Animate the circle conic section
  public void animateCircle()
  {
    // Define important values for the whole sequence
    Globals.coefA = 1.0;
    Globals.coefB = 0.0;
    Globals.coefC = 0.0;

    Globals.fullTravel = 4;
    Globals.step = (Globals.fullTravel * 2) / (Globals.frameCount - 1);

    //Circle conic section animation
    double coefD = (Globals.fullTravel);
    double edgeWidth = 30;
    bool isRotating = false;
    bool isGoingUp = false;
    translatePlane(coefD, edgeWidth, isGoingUp, isRotating);

    coefD = (0.75);
    isRotating = false;
    isGoingUp = true;
    translatePlane(coefD, edgeWidth, isGoingUp, isRotating);
  }



  //Animate the ellipse conic section
  public void animateEllipse()
  {
    // Define important values for the whole sequence
    Globals.coefA = 2.5;
    Globals.coefB = 1.0;
    Globals.coefC = 0.0;
    Globals.spinPoint = 0.5;

    Globals.fullTravel = 12;
    Globals.step = (Globals.fullTravel * 2) / (Globals.frameCount - 1);

    //Ellipse conic section animation
    double coefD = (Globals.fullTravel);
    double edgeWidth = 5;
    bool isRotating = false;
    bool isGoingUp = false;
    translatePlane(coefD, edgeWidth, isGoingUp, isRotating);

    coefD = (2.2);
    isRotating = true;
    isGoingUp = true;
    translatePlane(coefD, edgeWidth, isGoingUp, isRotating);
  }




  //Animate the parabola conic section
  public void animateParabola()
  {
    // Define important values for the whole sequence
    Globals.coefA = 1.0;
    Globals.coefB = 3.0;// tree has 1:3 ratio of its base radius to its height
    Globals.coefC = 0.0;
    Globals.spinPoint = 0.37;

    // extra length of fullTravel compensates for
    //LED creating the biggest cone around z axis
    Globals.fullTravel = 9;
    Globals.step = (Globals.fullTravel * 2) / (Globals.frameCount - 1);

    //Parabola conic section animation
    double coefD = (Globals.fullTravel / 2);
    double edgeWidth = 5;
    bool isRotating = false;
    bool isGoingUp = false;
    translatePlane(coefD, edgeWidth, isGoingUp, isRotating);

    coefD = (2.25);
    isRotating = true;
    isGoingUp = true;
    translatePlane(coefD, edgeWidth, isGoingUp, isRotating);
  }




  //Animate the hyperbola conic section
  public void animateHyperbola()
  {
    // Define important values for the whole sequence
    Globals.coefA = 0.0;
    Globals.coefB = 1.0;
    Globals.coefC = 0.0;
    Globals.spinPoint = 0.75;

    Globals.fullTravel = 2.5;
    Globals.step = (Globals.fullTravel * 2) / (Globals.frameCount - 1);

    //Hyperbola conic section animation
    double coefD = (Globals.fullTravel / 2);
    double edgeWidth = 30;
    bool isRotating = false;
    bool isGoingUp = false;
    translatePlane(coefD, edgeWidth, isGoingUp, isRotating);

    coefD = (Globals.fullTravel / 2);
    isRotating = true;
    isGoingUp = true;
    translatePlane(coefD, edgeWidth, isGoingUp, isRotating);
  }




  // Generate rows for each frame of the 3D plane translated trhough the tree
  public void translatePlane (double partCoefD, double edgeWidth, bool isGoingUp, bool isRotating)
  {
    //Generate rows
    for (int f = 0; f < (Globals.frameCount / 2); f++)
    {
      string row = f + ",";
      double coefD;

      //Determine the direction of the traversing plane
      if(!isGoingUp)
      {
        coefD = partCoefD - (Globals.step * f);
      }
      else
      {
        coefD = (Globals.step * f) - partCoefD;
      }

      //Determine if traversing plane will stop and rotate at some given spin-point
      if(isRotating && (f == Math.Round((double) (Globals.spinPoint * Globals.frameCount / 2))))
      {
        rotatePlane(coefD, edgeWidth, f);
      }

      // Calculate the values for each LED
      int r, g, b;
      for (int i = 0; i < Globals.ledCount; i++)
      {
        double ledToPlane = (Globals.leds[i].Z * Globals.coefA +
          Globals.leds[i].Y * Globals.coefB + Globals.leds[i].X * Globals.coefC);
        // Calculate RGB values for each LED
        if (ledToPlane < coefD)
        {
          r = 75 * (int) Math.Round(Math.Exp(-1 * edgeWidth * Math.Pow((coefD - ledToPlane), 2)));
          g = 75 * (int) Math.Round((-1 * Math.Exp(-1 * edgeWidth * Math.Pow((coefD - ledToPlane), 2)) + 1));
          b = 0;
        }

        else
        {
          r = 0;
          g = 0;
          b = 0;
        }

        // Add these colors to the row
        row += r + "," + g + "," + b;
        if (i != Globals.ledCount - 1)
        {
          row += ",";
        }
      }

      // Add this row to the CSV
      Globals.csv.Add(row);
    }
  }



  //Rotate the conic section 360 degrees before
  //the plane has completely traversed the tree
  public void rotatePlane (double coefD, double edgeWidth, int f)
  {
    stillFrames(coefD, edgeWidth, f);

    for (int j = 0; j > -180; j--)
    {
      string row = f + ",";

      double angle = j * (Math.PI / 180);

      //offset by 90 deg to start rotation on y axis. Given coefB to
      //maintain a circular rotation of the plane
      double newcoefC = ( Math.Cos(2 * angle + Math.PI / 2) * Globals.coefB);
      //offset of 90 deg to keep x and y out of phase
      double newcoefB = (Math.Sin(2 * angle + Math.PI / 2)) * Globals.coefB;

      int r, g, b;
      for (int t = 0; t < Globals.ledCount; t++)
      {
        double ledToPlane = (Globals.leds[t].Z * Globals.coefA +
          Globals.leds[t].Y * newcoefB + Globals.leds[t].X * newcoefC);

        // Calculate RGB values for each LED
        if (ledToPlane < coefD)
        {
          r = 75 * (int) Math.Round(Math.Exp(-1 * edgeWidth * Math.Pow((coefD - ledToPlane), 2)));
          g = 75 * (int) Math.Round((-1 * Math.Exp(-1 * edgeWidth * Math.Pow((coefD - ledToPlane), 2)) + 1));
          b = 0;
        }
        else
        {
          r = 0;
          g = 0;
          b = 0;
        }

        // Add these colors to the row
        row += r + "," + g + "," + b;
        if (t != Globals.ledCount - 1)
        {
          row += ",";
        }
      }

      // Add this row to the CSV
      Globals.csv.Add(row);
    }

    stillFrames(coefD, edgeWidth, f);
  }



  //Adds specified number of still frames to the CSV file
  public void stillFrames(double coefD, double edgeWidth, int f)
  {
    for(int u = 0; u < 20; u++)
    {
      string row = f + ",";

      int r, g, b;
      for (int p = 0; p < Globals.ledCount; p++)
      {
        double ledToPlane = (Globals.leds[p].Z * Globals.coefA +
          Globals.leds[p].Y * Globals.coefB + Globals.leds[p].X * Globals.coefC);

        // Calculate RGB values for each LED
        if (ledToPlane < coefD)
        {
          r = 75 * (int) Math.Round(Math.Exp(-1 * edgeWidth * Math.Pow((coefD - ledToPlane), 2)));
          g = 75 * (int) Math.Round((-1 * Math.Exp(-1 * edgeWidth * Math.Pow((coefD - ledToPlane), 2)) + 1));
          b = 0;
        }
        else
        {
          r = 0;
          g = 0;
          b = 0;
        }

        // Add these colors to the row
        row += r + "," + g + "," + b;
        if (p != Globals.ledCount - 1)
        {
          row += ",";
        }
      }

      // Add this row to the CSV
      Globals.csv.Add(row);
    }
  }
  // </Custom additional code> 

  private List<string> __err = new List<string>(); //Do not modify this list directly.
  private List<string> __out = new List<string>(); //Do not modify this list directly.
  private RhinoDoc doc = RhinoDoc.ActiveDoc;       //Legacy field.
  private IGH_ActiveObject owner;                  //Legacy field.
  private int runCount;                            //Legacy field.
  
  public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
  {
    //Prepare for a new run...
    //1. Reset lists
    this.__out.Clear();
    this.__err.Clear();

    this.Component = owner;
    this.Iteration = iteration;
    this.GrasshopperDocument = owner.OnPingDocument();
    this.RhinoDocument = rhinoDocument as Rhino.RhinoDoc;

    this.owner = this.Component;
    this.runCount = this.Iteration;
    this. doc = this.RhinoDocument;

    //2. Assign input parameters
        List<Point3d> leds = null;
    if (inputs[0] != null)
    {
      leds = GH_DirtyCaster.CastToList<Point3d>(inputs[0]);
    }
    int fps = default(int);
    if (inputs[1] != null)
    {
      fps = (int)(inputs[1]);
    }

    int duration = default(int);
    if (inputs[2] != null)
    {
      duration = (int)(inputs[2]);
    }



    //3. Declare output parameters
      object CSV = null;


    //4. Invoke RunScript
    RunScript(leds, fps, duration, ref CSV);
      
    try
    {
      //5. Assign output parameters to component...
            if (CSV != null)
      {
        if (GH_Format.TreatAsCollection(CSV))
        {
          IEnumerable __enum_CSV = (IEnumerable)(CSV);
          DA.SetDataList(1, __enum_CSV);
        }
        else
        {
          if (CSV is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(1, (Grasshopper.Kernel.Data.IGH_DataTree)(CSV));
          }
          else
          {
            //assign direct
            DA.SetData(1, CSV);
          }
        }
      }
      else
      {
        DA.SetData(1, null);
      }

    }
    catch (Exception ex)
    {
      this.__err.Add(string.Format("Script exception: {0}", ex.Message));
    }
    finally
    {
      //Add errors and messages... 
      if (owner.Params.Output.Count > 0)
      {
        if (owner.Params.Output[0] is Grasshopper.Kernel.Parameters.Param_String)
        {
          List<string> __errors_plus_messages = new List<string>();
          if (this.__err != null) { __errors_plus_messages.AddRange(this.__err); }
          if (this.__out != null) { __errors_plus_messages.AddRange(this.__out); }
          if (__errors_plus_messages.Count > 0) 
            DA.SetDataList(0, __errors_plus_messages);
        }
      }
    }
  }
}