import { useEffect, useState } from "react";
import { Button, Card, Col, Container, Row, Tab, Tabs } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import agent from "../lib/api/agent";

type FollowUser = {
  id: string;
  followerId: string;
  followingId: string;
  follower?: { userName: string };
  following?: { userName: string };
};

type UserType = {
  id: string;
  userName: string;
  email: string;
};

export default function UserPage() {
  const { user, isLoggedIn, isLoading } = useAuth();
  const [followers, setFollowers] = useState<FollowUser[]>([]);
  const [following, setFollowing] = useState<FollowUser[]>([]);
  const [allUsers, setAllUsers] = useState<UserType[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    if (isLoggedIn) {
      fetchFollowers();
      fetchFollowing();
      fetchAllUsers();
    }
  }, [isLoggedIn]);

  const fetchFollowers = async () => {
    try {
      const res = await agent.get("/follows/followers");
      setFollowers(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchFollowing = async () => {
    try {
      const res = await agent.get("/follows/following");
      setFollowing(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchAllUsers = async () => {
    try {
      const res = await agent.get("/auth/users");
      setAllUsers(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const handleFollow = async (userId: string) => {
    try {
      await agent.post(`/follows/${userId}`);
      fetchFollowing();
      fetchAllUsers();
    } catch (err) {
      console.log(err);
    }
  };

  const handleUnfollow = async (userId: string) => {
    try {
      await agent.delete(`/follows/${userId}`);
      fetchFollowing();
      fetchAllUsers();
    } catch (err) {
      console.log(err);
    }
  };

  const isFollowingUser = (userId: string) => {
    return following.some((f) => f.followingId === userId);
  };

  if (isLoading) {
    return (
      <Container className="py-5 text-center">
        <p>Loading...</p>
      </Container>
    );
  }

  if (!isLoggedIn) {
    return (
      <Container className="py-5">
        <Card className="auth-card mx-auto" style={{ maxWidth: "500px" }}>
          <Card.Title>Log in to view your profile</Card.Title>
          <div className="d-flex gap-2 justify-content-center mt-3">
            <Button variant="primary" onClick={() => navigate("/login")}>
              Log In
            </Button>
            <Button variant="outline-secondary" onClick={() => navigate("/signup")}>
              Sign Up
            </Button>
          </div>
        </Card>
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <Row>
        <Col md={10} lg={8} className="mx-auto">
          <Card className="user-profile-card mb-4">
            <Card.Body className="text-center py-4">
              <div className="user-avatar">
                {user?.userName?.charAt(0).toUpperCase()}
              </div>
              <h2 className="mt-3 mb-1">{user?.userName}</h2>
              <p className="text-muted">{user?.email}</p>
              <div className="user-stats d-flex justify-content-center gap-4 mt-3">
                <div>
                  <strong>{followers.length}</strong>
                  <span className="text-muted d-block">Followers</span>
                </div>
                <div>
                  <strong>{following.length}</strong>
                  <span className="text-muted d-block">Following</span>
                </div>
              </div>
            </Card.Body>
          </Card>

          <Card className="user-content-card">
            <Card.Body>
              <Tabs defaultActiveKey="following" className="mb-3">
                <Tab eventKey="following" title="Following">
                  {following.length === 0 ? (
                    <p className="text-muted text-center py-4">
                      You're not following anyone yet
                    </p>
                  ) : (
                    <div className="follow-list">
                      {following.map((f) => (
                        <div key={f.id} className="follow-item">
                          <div className="follow-avatar">
                            {f.following?.userName?.charAt(0).toUpperCase()}
                          </div>
                          <span className="follow-name">{f.following?.userName}</span>
                          <Button
                            variant="outline-danger"
                            size="sm"
                            onClick={() => handleUnfollow(f.followingId)}
                          >
                            Unfollow
                          </Button>
                        </div>
                      ))}
                    </div>
                  )}
                </Tab>
                <Tab eventKey="followers" title="Followers">
                  {followers.length === 0 ? (
                    <p className="text-muted text-center py-4">
                      No one is following you yet
                    </p>
                  ) : (
                    <div className="follow-list">
                      {followers.map((f) => (
                        <div key={f.id} className="follow-item">
                          <div className="follow-avatar">
                            {f.follower?.userName?.charAt(0).toUpperCase()}
                          </div>
                          <span className="follow-name">{f.follower?.userName}</span>
                        </div>
                      ))}
                    </div>
                  )}
                </Tab>
                <Tab eventKey="discover" title="Discover Users">
                  {allUsers.length === 0 ? (
                    <p className="text-muted text-center py-4">
                      No other users found
                    </p>
                  ) : (
                    <div className="follow-list">
                      {allUsers.map((u) => (
                        <div key={u.id} className="follow-item">
                          <div className="follow-avatar">
                            {u.userName?.charAt(0).toUpperCase()}
                          </div>
                          <span className="follow-name">{u.userName}</span>
                          {isFollowingUser(u.id) ? (
                            <Button
                              variant="outline-danger"
                              size="sm"
                              onClick={() => handleUnfollow(u.id)}
                            >
                              Unfollow
                            </Button>
                          ) : (
                            <Button
                              variant="primary"
                              size="sm"
                              onClick={() => handleFollow(u.id)}
                            >
                              Follow
                            </Button>
                          )}
                        </div>
                      ))}
                    </div>
                  )}
                </Tab>
              </Tabs>
            </Card.Body>
          </Card>

          <div className="text-center mt-4">
            <Button
              variant="primary"
              onClick={() => navigate("/create-recipe")}
            >
              Create New Recipe
            </Button>
          </div>
        </Col>
      </Row>
    </Container>
  );
}
