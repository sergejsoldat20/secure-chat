/* eslint-disable no-unused-vars */
import React, { useState, useEffect } from "react";
import { Button, Form, Input, Select, message } from "antd";
import { Grid } from "@mui/material";
import authenticate from "../../services/auth.service";
import { useNavigate } from "react-router-dom";

export default function Login() {
  const navigate = useNavigate();
  const [auth, setAuth] = useState({
    email: "",
    password: "",
  });

  const { email, password } = auth;

  const onInputChange = (e) => {
    // nastavlja da dodaje nove objekte
    setAuth({ ...auth, [e.target.name]: e.target.value });
  };

  const onFinishFailed = (errorInfo) => {};

  const onFinish = () => {
    authenticate.login(auth).then((result) => {
      if (result.status === 200) {
        localStorage.setItem("jwt", result.data.accessToken);
        navigate("/chat");
      } else {
        message.error("Password or username incorrect!");
      }
    });
  };
  return (
    <Grid alignItems="center" justifyContent="center" className="text-center">
      <div className="container">
        <div className="rom">
          <div className="col-md-6 offset-md-3 border rounder p-4 mt-2 shadow">
            <h2 className="text-center m-4">Login</h2>

            <div className="card">
              <div className="card-header">
                <Grid
                  container
                  spacing={0}
                  direction="column"
                  alignItems="center"
                  justifyContent="center"
                  paddingRight={12}
                >
                  <Form
                    name="basic"
                    labelCol={{
                      span: 10,
                    }}
                    wrapperCol={{
                      span: 20,
                    }}
                    initialValues={{
                      remember: true,
                    }}
                    onFinish={(e) => onFinish(e)}
                    onFinishFailed={onFinishFailed}
                    autoComplete="off"
                    requiredMark={false}
                  >
                    <Form.Item label="Email: ">
                      <Input
                        name="email"
                        value={email}
                        onChange={(e) => onInputChange(e)}
                        style={{ width: "300px" }}
                      />
                    </Form.Item>
                    <Form.Item label="Password: ">
                      <Input
                        name="password"
                        value={password}
                        onChange={(e) => onInputChange(e)}
                        style={{ width: "300px" }}
                      />
                    </Form.Item>

                    <Form.Item
                      wrapperCol={{
                        offset: 8,
                        span: 16,
                      }}
                    >
                      <Button
                        type="primary"
                        onClick={onFinish}
                        style={{ width: "120px" }}
                      >
                        Login
                      </Button>
                    </Form.Item>
                  </Form>
                </Grid>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Grid>
  );
}
