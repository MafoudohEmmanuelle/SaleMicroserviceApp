import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "./pages/Login";
import DashboardAdmin from "./pages/DashboardAdmin";
import DashboardEmployee from "./pages/DashboardEmployee";
import NotFound from "./pages/NotFound";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public route */}
        <Route path="/" element={<Login />} />

        {/* Admin routes */}
        <Route path="/admin/dashboard" element={<DashboardAdmin />} />

        {/* Employee routes */}
        <Route path="/employee/dashboard" element={<DashboardEmployee />} />

        {/* Fallback */}
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
