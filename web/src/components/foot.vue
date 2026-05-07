<template>
	<div class="foot">
		<!-- Start of Footer -->
		<footer id="footer-wrapper">
			<div id="footer" class="container">
				<div class="footer-brand">
					<div class="footer-brand-mark">KC</div>
					<div class="footer-brand-copy">
						<div class="footer-brand-title">KiwiConnect</div>
						<div class="footer-brand-subtitle">
							University of Waikato community hub
						</div>
					</div>
				</div>
				<div class="footer-grid">
					<div class="footer-column">
						<section class="widget">
							<h3 class="title">Main Features</h3>
							<div class="textwidget">
								<p>
									Ask questions, discover events, trade second-hand items,
									and stay connected with the Waikato student community.
								</p>
							</div>
						</section>
					</div>
					<div class="footer-column">
						<section class="widget">
							<h3 class="title">Community Notes</h3>
							<div class="textwidget">
								<p>
									Keep discussions respectful and avoid inappropriate content
									to maintain a safe student-friendly space.
								</p>
							</div>
						</section>
					</div>
					<div class="footer-column">
						<section class="widget">
							<h3 class="title">Customer Service</h3>
							<div class="footer-links" role="list">
								<router-link
									to="/feedback"
									title="Feedback"
									class="footer-link-card"
									role="listitem"
								>
									<span class="footer-link-label">Feedback</span>
									<span class="footer-link-arrow">↗</span>
								</router-link>
								<router-link
									to="/appeal"
									title="Account Appeal"
									class="footer-link-card"
									role="listitem"
								>
									<span class="footer-link-label">Account Appeal</span>
									<span class="footer-link-arrow">↗</span>
								</router-link>
								<router-link
									to="/report"
									title="Report Misconduct"
									class="footer-link-card"
									role="listitem"
								>
									<span class="footer-link-label">Report Misconduct</span>
									<span class="footer-link-arrow">↗</span>
								</router-link>
							</div>
						</section>
					</div>
				</div>
			</div>
			<!-- end of #footer -->
		</footer>
		<!-- End of Footer -->
	</div>
</template>

<script setup>
// Footer component - Vue 3
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.foot {
	width: 100%;
	bottom: 0;
}

#footer-wrapper {
	background:
		radial-gradient(circle at top left, rgba(38, 99, 235, 0.2), transparent 22%),
		linear-gradient(135deg, #1c49b7 0%, #2663eb 58%, #1749c6 100%);
	color: #dfe8ff;
	padding: 34px 0 26px;
	margin-top: 40px;
}

#footer {
	padding: 8px 0;
}

.footer-brand {
	display: flex;
	align-items: center;
	gap: 16px;
	margin-bottom: 18px;
	padding: 0;
}

.footer-brand-mark {
	width: 42px;
	height: 42px;
	border-radius: 14px;
	display: grid;
	place-items: center;
	background: linear-gradient(135deg, #ffffff, #dbe7ff);
	color: #2663eb;
	font-size: 18px;
	font-weight: 800;
	letter-spacing: 0.08em;
	box-shadow: 0 10px 20px rgba(19, 53, 137, 0.22);
}

.footer-brand-title {
	font-size: 18px;
	font-weight: 800;
	line-height: 1;
	color: #ffffff;
	letter-spacing: -0.03em;
}

.footer-brand-subtitle {
	margin-top: 4px;
	font-size: 12px;
	color: rgba(235, 242, 255, 0.82);
}

.row {
	display: flex;
	flex-wrap: wrap;
	gap: 24px;
}

.footer-grid {
	display: grid;
	grid-template-columns: 1.15fr 1.15fr 1fr;
	gap: 16px;
}

.widget {
	height: 100%;
	padding: 18px 20px;
	border-radius: 18px;
	background: rgba(255, 255, 255, 0.08);
	border: 1px solid rgba(255, 255, 255, 0.12);
	box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.03);
}

.widget .title {
	font-size: 15px;
	font-weight: 700;
	color: #fff;
	margin-bottom: 12px;
	padding-bottom: 8px;
	border-bottom: 1px solid rgba(255, 255, 255, 0.12);
}

.widget .textwidget {
	font-size: 13px;
	line-height: 1.65;
	color: #dfe8ff;
}

.footer-links {
	display: grid;
	gap: 10px;
}

.footer-link-card {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
	padding: 12px 14px;
	border-radius: 14px;
	background: rgba(255, 255, 255, 0.06);
	border: 1px solid rgba(255, 255, 255, 0.1);
	color: #edf3ff;
	text-decoration: none;
	font-size: 13px;
	font-weight: 600;
	transition:
		background 0.2s ease,
		border-color 0.2s ease,
		transform 0.2s ease,
		color 0.2s ease;
}

.footer-link-card:hover {
	transform: translateY(-1px);
	background: rgba(255, 255, 255, 0.12);
	border-color: rgba(255, 255, 255, 0.18);
	color: #fff;
}

.footer-link-label {
	line-height: 1.2;
}

.footer-link-arrow {
	flex: 0 0 auto;
	font-size: 14px;
	opacity: 0.78;
}

@media (max-width: 768px) {
	.footer-brand {
		align-items: flex-start;
	}

	.footer-grid {
		grid-template-columns: 1fr;
	}

	.widget {
		padding: 16px 18px;
	}
}
</style>
