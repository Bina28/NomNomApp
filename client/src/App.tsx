import { BrowserRouter, Route, Routes } from "react-router-dom";
import Nav from "./components/Nav";
import Recipes from "./components/Recipes";
import RecipeDetail from "./components/RecipeDetail";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import Login from "./components/Login";
import SignUp from "./components/SignUp";
import UserPage from "./components/UserPage";

function App() {
  return (
    <BrowserRouter>
      <Nav />
      <Routes>
        <Route path="/recipes" element={<Recipes />} />
        <Route path="/recipe/:id" element={<RecipeDetail />} />
        <Route path="/login" element={<Login />} />
       <Route path="/signup" element={<SignUp />} />
         <Route path="/user" element={<UserPage />} />

      </Routes>
    </BrowserRouter>
  );
}

export default App;
