import { useEffect, useState } from "react";
import { Alert, Button, Form } from "react-bootstrap";
import { useAuth } from "../context/AuthContext";
import agent from "../lib/api/agent";

type Comment = {
  id: string;
  text: string;
  score: number;
  createdAt: string;
  userName: string;
  userId: string;
};

type CommentsProps = {
  recipeId: number;
};

export default function Comments({ recipeId }: CommentsProps) {
  const [comments, setComments] = useState<Comment[]>([]);
  const [newComment, setNewComment] = useState("");
  const [score, setScore] = useState(5);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [averageScore, setAverageScore] = useState<number | null>(null);
  const { isLoggedIn, user } = useAuth();

  useEffect(() => {
    fetchComments();
    fetchAverageScore();
  }, [recipeId]);

  const fetchComments = async () => {
    try {
      const res = await agent.get(`/comments/recipe/${recipeId}`);
      setComments(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchAverageScore = async () => {
    try {
      const res = await agent.get(`/comments/recipe/${recipeId}/score`);
      setAverageScore(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newComment.trim()) return;

    setIsSubmitting(true);
    setError("");

    try {
      await agent.post("/comments", {
        recipeId: recipeId.toString(),
        text: newComment.trim(),
        score: score,
      });
      setNewComment("");
      setScore(5);
      fetchComments();
      fetchAverageScore();
    } catch (err: any) {
      setError(err.response?.data || "Could not add comment");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (commentId: string) => {
    try {
      await agent.delete(`/comments/${commentId}`);
      fetchComments();
      fetchAverageScore();
    } catch (err) {
      console.log(err);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("en-US", {
      day: "numeric",
      month: "short",
      year: "numeric",
    });
  };

  const renderStars = (rating: number) => {
    return "★".repeat(rating) + "☆".repeat(5 - rating);
  };

  return (
    <div className="comments-section">
      <div className="comments-header">
        <h3>Ratings and Comments</h3>
        {averageScore !== null && averageScore > 0 && (
          <div className="average-score">
            <span className="stars">{renderStars(Math.round(averageScore))}</span>
            <span className="score-text">{averageScore.toFixed(1)} / 5</span>
          </div>
        )}
      </div>

      {isLoggedIn ? (
        <Form onSubmit={handleSubmit} className="comment-form">
          {error && <Alert variant="danger">{error}</Alert>}
          <Form.Group className="mb-3">
            <Form.Label>Your Rating</Form.Label>
            <div className="star-rating">
              {[1, 2, 3, 4, 5].map((star) => (
                <span
                  key={star}
                  className={`star ${star <= score ? "active" : ""}`}
                  onClick={() => setScore(star)}
                >
                  ★
                </span>
              ))}
            </div>
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Label>Comment</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              placeholder="Write your comment here..."
              value={newComment}
              onChange={(e) => setNewComment(e.target.value)}
            />
          </Form.Group>
          <Button variant="primary" type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Submitting..." : "Add Comment"}
          </Button>
        </Form>
      ) : (
        <p className="login-prompt">
          <a href="/login">Log in</a> to add a comment
        </p>
      )}

      <div className="comments-list">
        {comments.length === 0 ? (
          <p className="no-comments">No comments yet. Be the first!</p>
        ) : (
          comments.map((comment) => (
            <div key={comment.id} className="comment-card">
              <div className="comment-header">
                <span className="comment-author">{comment.userName}</span>
                <span className="comment-stars">{renderStars(comment.score)}</span>
              </div>
              <p className="comment-text">{comment.text}</p>
              <div className="comment-footer">
                <span className="comment-date">{formatDate(comment.createdAt)}</span>
                {user?.id === parseInt(comment.userId) && (
                  <Button
                    variant="link"
                    size="sm"
                    className="delete-btn"
                    onClick={() => handleDelete(comment.id)}
                  >
                    Delete
                  </Button>
                )}
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
