import base from "./base.service";

const stringUser = "1y56t9z7-8s513sa6-7s03kl2-99p4";
const stringAdmin = "12-4a5ba8q-kkl9135d-dfhdc119-f33184";

const instance = base.service(false);
const authInstance = base.service(true);

export const setRole = (role) => {
  if (role === "USER") return stringUser;
  if (role === "ADMIN") return stringAdmin;
};

export const login = (authRequest) => {
  return instance.post("/Account/authenticate", authRequest);
};

export const register = (registerRequest) => {
  return instance.post("/Account/register", registerRequest);
};

export const getChatUsers = () => {
  return authInstance.get("/Account/users");
};

export const getCurrentUser = () => {
  return authInstance.get("/Account/current-user");
};

export const CheckIfAdmin = () => {
  const role = localStorage.getItem("role");
  if (role === stringAdmin) {
    return true;
  } else {
    return false;
  }
};

export const CheckIfUser = () => {
  const role = localStorage.getItem("role");
  if (role === stringUser) {
    return true;
  } else {
    return false;
  }
};

export const checkIfAuthorized = () => {
  if (localStorage.getItem("jwt") !== null) {
    return true;
  } else {
    return false;
  }
};

export default {
  login,
  register,
  getChatUsers,
  getCurrentUser,
  checkIfAuthorized,
};
