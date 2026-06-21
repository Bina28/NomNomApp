export type Recipe = {
  id: number;
  title: string;
  image: string;
  summary: string;
  instructions: string;
  extendedIngredients: string[];
};

export type Login = {
  email: string;
  password: string;
};

export type SignUp = {
  email: string;
  password: string;
  userName: string;
};

export type Comment = {
  id: string;
  text: string;
  score: number;
  createdAt: string;
  userName: string;
  userId: string;
};

export type CommentsProps = {
  recipeId: number;
};

export type Ingredient = {
  name: string;
  amount: string;
};


export type FollowUser = {
  id: string;
  followerId: string;
  followingId: string;
  follower?: { userName: string };
  following?: { userName: string };
};

export type UserType = {
  id: string;
  userName: string;
  email: string;
};


export type User = {
  id: string;
  email: string;
  userName: string;
};

export type AuthContextType = {
  user: User | null;
  isLoggedIn: boolean;
  isLoading: boolean;
  checkAuth: () => Promise<void>;
  logout: () => Promise<void>;
};

export type Notification = {
  id: number;
  title: string;
  message: string;
  type: "follow" | "comment";
};

export type SseContextType = {
  notifications: Notification[];
};