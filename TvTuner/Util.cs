namespace TvTuner {
    public static class Util {
        public static string PickColor(int rating) {
            if (rating > 75) {
                return "green";
            }
            if (rating > 50) {
                return "yellow";
            }
            if (rating > 25) {
                return "orange";
            } else {
                return "red";
            }
        }
    }
}