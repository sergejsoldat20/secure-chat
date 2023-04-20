/* eslint-disable no-unused-vars */
import React, { useState, useEffect } from "react";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { Button, Form, Input, message } from "antd";
import { GlobalStyles } from "@mui/system";
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

  const onFinish = (values) => {
    console.log(auth);
    authenticate.login(values).then((result) => {
      if (result.status === 200) {
        localStorage.setItem("jwt", result.data.accessToken);
        navigate("/chat");
      } else {
        message.error("Password or username incorrect!");
      }
    });
  };
  return (
    <Grid
      container
      spacing={0}
      direction="column"
      alignItems="center"
      justifyContent="center"
    >
      <div style={{ height: 50 }}></div>
      <div className="col-md-4 border rounder p-4 mt-2 shadow">
        <Form
          name="normal_login"
          className="login-form col-md-12 text-center p-1"
          initialValues={{ remember: true }}
          onFinish={(e) => onFinish(e)}
        >
          <GlobalStyles styles={{ Form: { width: 100, height: 150 } }} />
          <Form.Item
            style={{
              width: "100%",
            }}
            name="email"
            // value={email}
            // onChange={(e) => onInputChange(e)}
          >
            <Input
              prefix={<UserOutlined className="site-form-item-icon" />}
              placeholder="Email"
            />
          </Form.Item>
          <Form.Item
            style={{
              width: "100%",
            }}
            name="password"
          >
            <Input
              prefix={<LockOutlined className="site-form-item-icon" />}
              type="password"
              // value={password}
              // onChange={(e) => onInputChange(e)}
              placeholder="Password"
            />
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              className="login-form-button md-4"
              onClick={onFinish}
            >
              Sign in
            </Button>
          </Form.Item>
        </Form>
      </div>
    </Grid>
  );
}
