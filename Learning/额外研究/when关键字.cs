using System.Net.Http;
using System.Threading.Tasks;

namespace TigerLearning.Learning.额外研究;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
public class when关键字 {
    /// <summary>
    /// 关于when在try-catch中的应用(.net6.0)
    /// </summary>
    public static async Task<string> MakeRequest() {
        var client = new HttpClient();
        var streamTask = client.GetStringAsync("https://localHost:10000");
        try {
            var responseText = await streamTask;
            return responseText;
        }
        catch(HttpRequestException e) when(e.Message.Contains("404")) {
            return "Page Not Found";
        }
        catch(HttpRequestException e) when(e.Message.Contains("301")) {
            return "Site Moved";
        }
        catch(HttpRequestException e) {
            return e.Message;
        }
    }

    /// <summary>
    /// when在switch-case中的应用(.net7.0?)
    /// </summary>
    public static void DisplayMeasurements(int a, int b) {
        switch((a, b)) {
        case ( > 0, > 0) when a == b:
            Console.WriteLine($"Both measurements are valid and equal to {a}.");
            break;
        case ( > 0, > 0):
            Console.WriteLine($"First measurement is {a}, second measurement is {b}.");
            break;
        default:
            Console.WriteLine("One or both measurements are not valid.");
            break;
        }
    }

    /// <summary>
    /// when在switch表达式中的应用
    /// </summary>
    public static Point Transform(Point point) => point switch {
        { X: 0, Y: 0 } => new Point(0, 0),
        { X: var x, Y: var y } when x < y => new Point(x + y, y),
        { X: var x, Y: var y } when x > y => new Point(x - y, y),
        { X: var x, Y: var y } => new Point(2 * x, 2 * y),
    };
}