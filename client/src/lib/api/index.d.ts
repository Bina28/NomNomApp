type Recipes = {
  id: number;
  title: string;
  image: string;
};

type Recipe = {
  title: string;
  summary: string;
  instructions: string;
  extendedIngredients: string[];
  image: string;
};

type Login = {
  email: string;
  password:string;
}

type SignUp = {
  email: string;
  password:string;
  userName: string;
}